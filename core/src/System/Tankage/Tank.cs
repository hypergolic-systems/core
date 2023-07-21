using Hgs.Core.Virtual;

namespace Hgs.Core.System.Tankage;

public class Tank : VirtualPart {

  private TankedSubstance substance = TankedSubstance.Empty;
  private float amount = 0;
  private float volume = 0;

  public Tank(uint partId, uint index) : base(partId, index) {}

  public TankedSubstance GetSubstance() {
    return this.substance;
  }

  public float GetAmount() {
    return this.amount;
  }

  public float GetVolume() {
    return this.volume;
  }

  public override void Load(object node) {
    base.Load(node);
    this.amount = float.Parse(Adapter.ConfigNode_Get(node, "amount"));
    this.volume = float.Parse(Adapter.ConfigNode_Get(node, "volume"));
  }

  public override void Save(object node) {
    Adapter.ConfigNode_Set(node, "amount", this.amount.ToString());
    Adapter.ConfigNode_Set(node, "volume", this.volume.ToString());
    base.Save(node);
  }
}
