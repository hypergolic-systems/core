using Hgs.System.Electrical;
using System.Collections.Generic;

namespace Hgs.Virtual {
  public class VirtualVessel {
    public Dictionary<uint, List<VirtualPart>> virtualPartsMap = new Dictionary<uint, List<VirtualPart>>();
    public Bus highVoltageBus = new Bus(Voltage.High);
    public Bus lowVoltageBus = new Bus(Voltage.Low);

    public Vessel liveVessel;

    public VirtualVessel(Vessel liveVessel) {
      lowVoltageBus.AddProducer(new LvFromHvLink(highVoltageBus));
      this.liveVessel = liveVessel;
    }

    public void Tick(uint seconds) {
      this.highVoltageBus.PreTick(seconds, this);
      this.lowVoltageBus.PreTick(seconds, this);
      this.highVoltageBus.Tick(seconds, this);
      this.lowVoltageBus.Tick(seconds, this);

      foreach (var parts in virtualPartsMap.Values) {
        foreach (var part in parts) {
        if (part.liveModule != null) {
          part.liveModule.OnSimulationUpdate(seconds);
        }
        }
      }
    }

    public void PostTick(uint seconds) {

    }
  }
}
