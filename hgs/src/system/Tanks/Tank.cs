using Hgs.Part;
using System.Collections.Generic;

namespace Hgs.Virtual.Tanks {

  public enum TankedSubstance {
    Empty,
    RocketFuel,
    LiquidOxygen,
    HypergolicFuel,
    XenonGas,
  }

  public class Tank : VirtualPart {

    public TankedSubstance substance = TankedSubstance.Empty;
    public float amount = 0;
    public float volume = 0;

    public Tank(uint partId, uint index) : base(partId, index) {}

    public override void Load(ConfigNode node) {
      base.Load(node);
    }

    public override void Save(ConfigNode node) {
      base.Save(node);
    }
  }
}