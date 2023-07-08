using Hgs.Part;
using Hgs.Virtual.Electrical;
using System.Collections.Generic;
using UnityEngine;

namespace Hgs.Virtual {
  public class VirtualVessel {
    public Dictionary<uint, List<VirtualPart>> virtualPartsMap = new Dictionary<uint, List<VirtualPart>>();
    public Bus highVoltageBus = new Bus(Voltage.High);
    public Bus lowVoltageBus = new Bus(Voltage.Low);

    public Vessel vessel;

    public VirtualVessel(Vessel vessel) {
      this.vessel = vessel;
      lowVoltageBus.AddProducer(new LvFromHvLink(highVoltageBus));
    }

    public void Tick(uint seconds) {
      this.highVoltageBus.PreTick(seconds, vessel);
      this.lowVoltageBus.PreTick(seconds, vessel);
      this.highVoltageBus.Tick(seconds, vessel);
      this.lowVoltageBus.Tick(seconds, vessel);

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
