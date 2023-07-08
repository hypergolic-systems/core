using System;
using System.Collections.Generic;
using Hgs.Virtual;

namespace Hgs.System.Electrical {

  public class Bus : SimulatedSystem {
    public Voltage Voltage;
    public Voltage GetVoltage() {
      return this.Voltage;
    }


    private List<PowerProducer> producers = new List<PowerProducer>();
    private List<PowerConsumer> consumers = new List<PowerConsumer>();
    private List<PowerStorage> storage = new List<PowerStorage>();

    private Cursor<PowerProducer> cursorProducer;

    public Bus(Voltage Voltage) {
      this.Voltage = Voltage;
      cursorProducer = new Cursor<PowerProducer>(producers);
    }

    public int TryDrawPower(int wattsNeeded) {
      return TryDrawPower(wattsNeeded, /* storageAllowed */ true);
    }

    public int TryDrawPower(int wattsNeeded, bool storageAllowed) {
      int drawn = 0;
      while (wattsNeeded > 0 && cursorProducer.Current != null && (storageAllowed || !(cursorProducer.Current is PowerStorage))) {
        int drawnFromThisProducer = cursorProducer.Current.TryDrawPower(wattsNeeded);
        if (drawnFromThisProducer == 0) {
          cursorProducer.Advance();
        }
        drawn += drawnFromThisProducer;
        wattsNeeded -= drawnFromThisProducer;
      }
      return drawn;
    }

    public void AddProducer(PowerProducer producer) {
      if (producer.GetVoltage() != Voltage) {
        throw new Exception("Wrong system voltage for producer");
      }
      producers.Add(producer);
    }

    public void AddConsumer(PowerConsumer load) {
      this.consumers.Add(load);
    }

    public void AddStorage(PowerStorage storage) {
      this.producers.Add(storage);
      this.storage.Add(storage);
    }

    public void PreTick(uint delta, VirtualVessel vessel) {
      foreach (PowerProducer producer in this.producers) {
        producer.OnCalculateProduction(delta, vessel);
      }
      foreach (PowerConsumer consumer in this.consumers) {
        consumer.OnCalculateDemand(delta);
      }
      this.cursorProducer.Reset();
    }

    public void Tick(uint delta, VirtualVessel vessel) {
      foreach (PowerConsumer consumer in this.consumers) {
        consumer.OnPowerAvailable(this);
      }

      if (this.cursorProducer.Current != null && !(this.cursorProducer.Current is PowerStorage)) {
        this.TryRechargeStorage();
      }
    }

    void TryRechargeStorage() {
      foreach (PowerStorage storage in this.storage) {
        int wattsNeeded = storage.GetWattsCapacity() - storage.GetWattsStored();
        int rechargeWatts = TryDrawPower(wattsNeeded, false);
        if (rechargeWatts == 0) {
          // No power available.
          break;
        }
      
        wattsNeeded -= rechargeWatts;
        storage.OnRecharge(rechargeWatts);
      }
    }

    public void PostTick(uint delta, VirtualVessel vessel) {}

    class Cursor<T> where T: class {
      List<T> source;
      List<T>.Enumerator position;

      public Cursor(List<T> source) {
        this.source = source;
      }

      private T _current = null;

      public T Current { 
        get {
          return this._current;
        }
      }

      public void Reset() {
        position = source.GetEnumerator();
        Advance();
      }

      public void Advance() {
        if (!position.MoveNext()) {
          _current = null;
        } else {
          _current = position.Current;
        }
      }
    }
  }
}
