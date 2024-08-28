using System;
using System.Collections.Generic;
using System.Linq;
using Hgs.Core.Simulator;
using Hgs.Core.Virtual;

namespace Hgs.Core.Resources;

public class ResourceSystem : ISimulated {

  private HashSet<IProducer> producers = new();
  private HashSet<IBuffer> buffers = new();
  private HashSet<Ticket> tickets = new();

  // TODO: perhaps we should cache this?
  public ulong RemainingValidDeltaT {
    get => Math.Min(tickets.Select(t => t.RemainingValidDeltaT).DefaultIfEmpty(ulong.MaxValue).Min(),
                    Math.Min(producers.Select(p => p.RemainingValidDeltaT).DefaultIfEmpty(ulong.MaxValue).Min(),
                             buffers.Select(remainingDeltaTOfBuffer).DefaultIfEmpty(ulong.MaxValue).Min()));
  }

  public void OnSynchronized() {
    // Nothing needs to happen here.
  }

  public void AddProducer(IProducer producer) {
    producers.Add(producer);
  }

  public void AddBuffer(IBuffer buffer) {
    buffers.Add(buffer);
  }

  private ulong remainingDeltaTOfBuffer(IBuffer buffer) {
    if (buffer.Rate > 0) {
      // Buffer is filling, so we want the time until full.
      return (ulong) Math.Ceiling((buffer.Capacity - buffer.Amount) / buffer.Rate);
    } else if (buffer.Rate < 0) {
      // Buffer is draining, so we want the time until empty.
      return (ulong) Math.Ceiling(buffer.Amount / -buffer.Rate);
    } else {
      // Buffer is stable, so we can keep it as is.
      return ulong.MaxValue;
    }
  }

  public Ticket NewTicket() {
    var ticket = new Ticket();
    tickets.Add(ticket);
    return ticket;
  }

  public void RecomputeState() {
    // Start off by zeroing all production and consumption, in preparation for a new round of resource allocation.
    foreach (var producer in producers) {
      producer.DynamicProductionRate = 0;
    }
    foreach (var buffer in buffers) {
      buffer.Rate = 0;
    }
    foreach (var ticket in tickets) {
      ticket.Rate = 0;
    }

    var remainingTickets = new Queue<Ticket>(tickets.Where(t => t.Request > 0));

    // Start out with the total baseline production, as it's always available.
    var produced = producers.Select(p => p.BaselineProduction).Sum();
    while (produced > 0 && remainingTickets.Count > 0) {
      var ticket = remainingTickets.Peek();
      if (produced >= ticket.Request) {
        ticket.Rate = ticket.Request;
        produced -= ticket.Request;
        remainingTickets.Dequeue();
      } else {
        ticket.Rate += produced;
        produced = 0;
      }
    }

    // Split the producers into prioritized groups, ordered by priority.
    var dynamicProducersList = producers.Where(p => p.DynamicProductionLimit > 0).GroupBy(p => p.Priority).ToList();
    dynamicProducersList.Sort((a, b) => a.Key.CompareTo(b.Key));

    // We actually want a Queue of producers, so we can iterate through in order. Each group of
    // producers of the given priority is stored as a `HashSet`, so we can easily remove producers
    // as they're used up.
    var dynamicProducers = new Queue<HashSet<IProducer>>(dynamicProducersList.Select(producersInGroup => new HashSet<IProducer>(producersInGroup)));
    var dynamicProducersIter = dynamicProducers.GetEnumerator();

    var group = dynamicProducersIter.MoveNext() ? dynamicProducersIter.Current : null;
    while (group != null && remainingTickets.Count > 0) {
      var ticket = remainingTickets.Peek();
      trySatisfyTicketFromProducers(ticket, group);

      // Check if the ticket is fully satisfied.
      if (ticket.Rate >= ticket.Request) {
        remainingTickets.Dequeue();
      }

      // We want to skip any empty groups, to ensure `group` always points to a producer group with
      // available production capacity.
      while (group != null && group.Count == 0) {
        group = dynamicProducersIter.MoveNext() ? dynamicProducersIter.Current : null;
      }
    }


    // Now that we've satisfied as many tickets as we can, we can satisfy further tickets by taking
    // from buffers.

    while (remainingTickets.Count > 0) {
      // Here we dequeue tickets instead of peeking, since this is the _last chance_ for them to be
      // fulfilled.
      var ticket = remainingTickets.Dequeue();
      var eligibleBuffers = new HashSet<IBuffer>(buffers.Where(b => b.Amount > 0 && (ticket.BufferFilter == null || ticket.BufferFilter(b))));

      while (ticket.Rate < ticket.Request && eligibleBuffers.Count > 0) {
        trySatisfyTicketFromBuffers(ticket, eligibleBuffers);
      }
    }

    // At this point, we've satisfied all the tickets as best as we can with the current state of
    // resource production & buffers. Even if we didn't satisfy all tickets, there may still be
    // production available (due to ticket filters) that can be buffered.
    //
    // Note that we add `b.Rate` here since a buffer that's currently being drained can accept
    // more production, even if it's already at capacity.
    var buffersWithSpace = new HashSet<IBuffer>(buffers.Where(b => b.Amount + b.Rate < b.Capacity));
    while (group != null && buffersWithSpace.Count > 0) {
      tryStoreExcessProduction(group, buffersWithSpace);

      while (group.Count == 0) {
        group = dynamicProducersIter.MoveNext() ? dynamicProducersIter.Current : null;
      }
    }

    // At this point, the resource flows we've derived have reached a steady state. Commit them so
    // that nodes can react to these changes.
    foreach (var producer in producers) {
      producer.Commit();
    }
    foreach (var buffer in buffers) {
      buffer.Commit();
    }
  }


  private void trySatisfyTicketFromProducers(Ticket ticket, HashSet<IProducer> producers) {
    var remaining = ticket.Request - ticket.Rate;

    // Some `producers` in producers may have a lower max production than the others, so we need to
    // determine the minimum production rate they all can sustain.
    var minPerProducer = producers.Select(p => p.DynamicProductionLimit - p.DynamicProductionRate).Min();

    // Because there is demand, we're definitely taking from each producer. We can take up to
    // `minPerProducer`.
    var perProducer = Math.Min(minPerProducer, remaining / producers.Count);
    foreach (var producer in producers) {
      producer.DynamicProductionRate += perProducer;
    }

    // Increase the rate given to the ticket by the amount we've taken from the producers.
    ticket.Rate += perProducer * producers.Count;

    // Remove any producers that have reached their limit.
    producers.RemoveWhere(p => p.DynamicProductionRate >= p.DynamicProductionLimit);

    // That's all we need to do here. If the ticket is fully satisifed, it'll be removed from the
    // queue by the caller. If not, we'll be seeing this ticket again, with a new set of producers
    // that are able to satisfy more of its demand.
  }

  private void trySatisfyTicketFromBuffers(Ticket ticket, HashSet<IBuffer> buffers) {
    var remaining = ticket.Request - ticket.Rate;

    // Buffers can only deliver until they run out. Too large of a rate will drain the buffer before
    // the next simulation step, so we need to determine the minimum rate they can sustain for at 
    // least the next second, since the simulation runs at a maximum rate of 1 Hz.
    //
    // The maximum rate a buffer can sustain for the next second is the amount it contains.
    var minPerBuffer = buffers.Select(b => b.Amount + b.Rate).Min();

    // Because there is demand, we're definitely taking from each buffer. We can take up to
    // `minPerBuffer`.
    var perBuffer = Math.Min(minPerBuffer, remaining / buffers.Count);
    foreach (var buffer in buffers) {
      // Note: consumtion is a negative rate for a buffer.
      buffer.Rate -= perBuffer;
    }

    // Increase the rate given to the ticket by the amount we've taken from the buffers.
    ticket.Rate += perBuffer * buffers.Count;

    // Remove any buffers that have reached their rate limit.
    buffers.RemoveWhere(b => -b.Rate >= b.Amount);

    // That's all we need to do here. If the ticket is not fully satisfied and the buffer list is
    // not empty, we'll be seeing this ticket again.
  }

  private void tryStoreExcessProduction(HashSet<IProducer> producers, HashSet<IBuffer> buffers) {
    // We have two potential limits:
    // * We shouldn't allocate more than the most constrained producer can produce.
    // * We shouldn't allocate more than the most constrained buffer can store.
    var minPerProducer = producers.Select(p => p.DynamicProductionLimit - p.DynamicProductionRate).Min();
    var minPerBuffer = buffers.Select(b => b.Capacity + b.Rate - b.Amount).Min();

    // We will modify rates according to the lower of the two constraints.
    var minDelta = Math.Min(minPerProducer, minPerBuffer);

    foreach (var buffer in buffers) {
      buffer.Rate += minDelta;
    }
    foreach (var producer in producers) {
      producer.DynamicProductionRate += minDelta;
    }

    buffers.RemoveWhere(b => b.Amount + b.Rate >= b.Capacity);
    producers.RemoveWhere(p => p.DynamicProductionRate >= p.DynamicProductionLimit);
  }

  public void Tick(ulong deltaT) {

    foreach (var producer in producers) {
      producer.Tick(deltaT);
    }

    foreach (var buffer in buffers) {
      buffer.Amount += buffer.Rate * deltaT;
    }

    foreach (var ticket in tickets) {
      ticket.FireOnTick(deltaT);
    }
  }

  public void OnStabilized() {
    foreach (var ticket in tickets) {
      ticket.FireOnCommit();
    }
  }

  public interface IProducer {

    int Priority { get; }
    float BaselineProduction { get; }
    float DynamicProductionLimit { get; }
    float DynamicProductionRate { get; set; }
    ulong RemainingValidDeltaT { get; }

    void Tick(ulong deltaT);
    void Commit();
  }

  public interface IBuffer {
    float Amount { get; set; }
    float Capacity { get; }
    float Rate { get; set; }

    void Commit();
  }

  public class Ticket {
    public VirtualComponent Owner;

    // Resource demand in units per second.

    private float request = 0;
    public float Request {
      get => request;
      set {
        if (request != value) {
          RemainingValidDeltaT = 0;
        }
        request = value;
      }
    }

    public float Rate = 0;

    public ulong RemainingValidDeltaT = ulong .MaxValue;


    public event OnCommitFn OnCommit;
    public event OnTickFn OnTick;

    public BufferFilterFn BufferFilter;

    public delegate void OnCommitFn();
    public delegate void OnTickFn(ulong deltaT);
    public delegate bool BufferFilterFn(IBuffer buffer);

    internal void FireOnCommit() {
      if (OnCommit != null) {
        OnCommit();
      }
    }

    internal void FireOnTick(ulong deltaT) {
      if (OnTick != null) {
        OnTick(deltaT);
      }
    }
  }
}
