using System;
using System.Collections.Generic;
using Hgs.Core.Virtual;
using System.Linq;
using System.Diagnostics;

namespace Hgs.Core.System.Electrical;

public class Bus : SimulatedSystem {
  public Voltage voltage;

  protected PowerComponent[] components;
  protected PowerComponent[][] producerGroups = new PowerComponent[10][];
  protected PowerComponent[][] consumerGroups = new PowerComponent[10][];

  protected PowerComponent[] storageComponents;

  public static Bus FromSpacecraft(CompositeSpacecraft composite, Voltage voltage) {
    var components = composite.partMap.Values.SelectMany(p => p.components).OfType<PowerComponent>().ToArray();
    var storageComponents = components.Where(c => c.Type == PowerComponentType.Storage).ToArray();
    var bus = new Bus {
      voltage = voltage,
      components = components,
      storageComponents = storageComponents,
    };

    // The 0th index is not used, but we need to initialize it to avoid null checks.
    bus.producerGroups[0] = bus.consumerGroups[0] = new PowerComponent[0];

    for (var priority = 1; priority < 10; priority++) {
      bus.producerGroups[priority] = components.Where(c => c.Type == PowerComponentType.Producer && c.Priority == priority).ToArray();
      bus.consumerGroups[priority] = components.Where(c => c.Type == PowerComponentType.Consumer && c.Priority == priority).ToArray();
    }

    return bus;
  }

  public void Tick(uint seconds, CompositeSpacecraft vessel) {
    var producerCursor = producerGroups.Where(p => p.Length > 0).GetEnumerator();
    PowerComponent[] currentProducers = null;
    if (producerCursor.MoveNext()) {
      currentProducers = producerCursor.Current;
    }

    foreach (var consumers in consumerGroups.Where(c => c.Length > 0)) {
      while (currentProducers != null) {
        // Try to power these consumers from the current producers.
        var poweredAllConsumers = ProcessPowerStep(currentProducers, consumers);
        if (poweredAllConsumers) {
          // We've met all demand in this group.
          break;
        }

        // We didn't meet all demand in this group, so we need to try the next group of producers.
        if (!producerCursor.MoveNext()) {
          // We've run out of producers.
          currentProducers = null;
          break;
        }
        currentProducers = producerCursor.Current;
      }

      if (currentProducers == null) {
        // We ran out of producers without powering all the consumers. Try to power them from
        // stored power next.
        ProcessPowerStep(storageComponents, consumers);
      }
    }

    while (currentProducers != null) {
      // We still have producers left over. Try to store excess power. Note that we don't care
      // about the return value here - producers may not be able to fully charge storage in one
      // tick.
      ProcessPowerStep(currentProducers, storageComponents);

      // Advance to the next set of producers, if any.
      currentProducers = producerCursor.MoveNext() ? producerCursor.Current : null;
    }

    producerCursor.Dispose();
  }

  protected bool ProcessPowerStep(PowerComponent[] producers, PowerComponent[] consumers) {
    var activeProducers = new HashSet<PowerComponent>(producers);
    var activeConsumers = new HashSet<PowerComponent>(consumers.Where(c => c.Demand > 0));
    while (true) {
      var demand = consumers.Select(c => c.Demand).Sum();

      if (demand == 0) {
        // We've met all demand in this group.
        return true;
      } else if (activeProducers.Count == 0) {
        return false;
      }

      // Attempt to produce an average amount of power per active producer.
      var perProducer = Math.Min(demand / activeProducers.Count, 1);
      var production = 0;
      foreach (var producer in activeProducers) {
        var produced = producer.PowerOut(perProducer);
        production += produced;

        if (production == demand) {
          // We've met the goal.
          break;
        } else if (produced == 0) {
          // This producer can't produce any more power.
          activeProducers.Remove(producer);
        }
      }

      if (production == 0) {
        // We've run out of producers capable of producing power.
        return false;
      }

      while (production > 0) {
        var perConsumer = Math.Min(production / activeConsumers.Count, 1);
        foreach (var consumer in activeConsumers) {
          var drain = consumer.PowerIn(perConsumer);
          production -= drain;
          if (consumer.Demand == 0) {
            activeConsumers.Remove(consumer);
          }
          if (production == 0) {
            // We've run out of power to distribute.
            break;
          }
        }
      }
    }
  }

  public void PreTick(uint seconds, CompositeSpacecraft vessel) {
    foreach (var component in components) {
      component.PowerPrepare(seconds);
    }
  }

  public void PostTick(uint seconds, CompositeSpacecraft vessel) {
    foreach (var component in components) {
      component.PowerFinished(seconds);
    }
  }
}
