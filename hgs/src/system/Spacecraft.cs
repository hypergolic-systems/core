using Hgs.System.Electrical;
using System.Collections.Generic;

namespace Hgs.System {
  public class Spacecraft {
    public Dictionary<uint, SimulatedPart> parts = new Dictionary<uint, SimulatedPart>();
    public Bus highVoltageBus = new Bus(Voltage.High);
    public Bus lowVoltageBus = new Bus(Voltage.Low);

    public Vessel vessel;

    public Spacecraft(Vessel vessel) {
      this.vessel = vessel;
      lowVoltageBus.AddProducer(new LvFromHvLink(highVoltageBus));
    }

    public void AddPart(SimulatedPart part) {
      parts[part.partId] = part;
    }

    public void Tick(uint seconds) {
      this.highVoltageBus.PreTick(seconds, vessel);
      this.lowVoltageBus.PreTick(seconds, vessel);
      this.highVoltageBus.Tick(seconds, vessel);
      this.lowVoltageBus.Tick(seconds, vessel);

      foreach (var part in parts.Values) {
        if (part.simModule != null) {
          part.simModule.OnSimulationUpdate(seconds);
        }
      }
    }

    public void PostTick(uint seconds) {

    }
  }
}
