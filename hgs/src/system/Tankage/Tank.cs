using Hgs.Virtual;

namespace Hgs.System.Tankage {
  public class Tank : VirtualPart {

    public TankedSubstance substance = TankedSubstance.Empty;
    public float amount = 0;
    public float volume = 0;

    public Tank(uint partId, uint index) : base(partId, index) {}

    public override void Load(ConfigNode node) {
      base.Load(node);
    }

    public override void Save(ConfigNode node) {
      node.AddValue("amount", this.amount);
      node.AddValue("volume", this.volume);
      base.Save(node);
    }
  }
}