using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hgs.System;

namespace Hgs.System.Electrical
{

  public class Bus : IBus, ISimulated {
    public Voltage Voltage;
    public Voltage GetVoltage() {
      return this.Voltage;
    }


    private List<IProducer> producers = new List<IProducer>();
    private List<IConsumer> consumers = new List<IConsumer>();
    private List<IStorage> storage = new List<IStorage>();

    private Cursor<IProducer> cursorProducer;

    public Bus(Voltage Voltage) {
      this.Voltage = Voltage;
      cursorProducer = new Cursor<IProducer>(producers);
    }

    public int TryDrawPower(int wattsNeeded) {
      return TryDrawPower(wattsNeeded, /* storageAllowed */ true);
    }

    public int TryDrawPower(int wattsNeeded, bool storageAllowed) {
      Debugger.Break();
      Console.WriteLine("Consumer has asked for {0}", wattsNeeded);
      int drawn = 0;
      while (wattsNeeded > 0 && cursorProducer.Current != null && (storageAllowed || !(cursorProducer.Current is IStorage))) {
        int drawnFromThisProducer = cursorProducer.Current.TryDrawPower(wattsNeeded);
        if (drawnFromThisProducer == 0) {
          cursorProducer.Advance();
        }
        drawn += drawnFromThisProducer;
        wattsNeeded -= drawnFromThisProducer;
        Console.WriteLine("Drew {0} from a producer, {1} so far", drawnFromThisProducer, drawn);
      }
      return drawn;
    }

    public void AddProducer(IProducer producer) {
      if (producer.GetVoltage() != Voltage)
      {
        throw new Exception("Wrong system voltage for producer");
      }
      producers.Add(producer);
    }

    public void AddConsumer(IConsumer load) {
      this.consumers.Add(load);
    }

    public void AddStorage(IStorage storage) {
      this.producers.Add(storage);
      this.storage.Add(storage);
    }

    public void PreTick(uint delta) {
      foreach (IProducer producer in this.producers) {
        producer.OnCalculateProduction(delta);
      }
      foreach (IConsumer consumer in this.consumers) {
        consumer.OnCalculateDemand(delta);
      }
      this.cursorProducer.Reset();
    }

    public void Tick(uint delta) {
      foreach (IConsumer consumer in this.consumers) {
        consumer.OnPowerAvailable(this);
      }

      if (this.cursorProducer.Current != null && !(this.cursorProducer.Current is IStorage)) {
        this.TryRechargeStorage();
      }
    }

    void TryRechargeStorage() {
      foreach (IStorage storage in this.storage) {
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

    public void PostTick(uint delta) {}

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
