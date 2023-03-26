using Hgs.System.Electrical;
using System.Collections.Generic;

namespace Hgs.System {
  public class Spacecraft {
    public Dictionary<uint, SimulatedPart> parts = new Dictionary<uint, SimulatedPart>();
    public Bus highVoltageBus = new Bus(Voltage.High);
    public Bus lowVoltageBus = new Bus(Voltage.Low);

    public Spacecraft() {
      lowVoltageBus.AddProducer(new LvFromHvLink(highVoltageBus));
    }

    public void AddPart(SimulatedPart part) {
      parts[part.partId] = part;
    }

    public void Tick(uint seconds) {
      this.highVoltageBus.PreTick(seconds);
      this.lowVoltageBus.PreTick(seconds);
      this.highVoltageBus.Tick(seconds);
      this.lowVoltageBus.Tick(seconds);

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
