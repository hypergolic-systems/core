using System;
using Hgs.Core.Virtual;

namespace Hgs.Core.System.Tankage;

public class Tank : VirtualComponent {

  private TankedSubstance substance = TankedSubstance.Empty;
  private float amount = 0;
  private float volume = 0;

  public TankedSubstance GetSubstance() {
    return this.substance;
  }

  public float GetAmount() {
    return this.amount;
  }

  public float GetVolume() {
    return this.volume;
  }

  protected override void Load(object node) {
    this.amount = float.Parse(Adapter.ConfigNode_Get(node, "amount"));
    this.volume = float.Parse(Adapter.ConfigNode_Get(node, "volume"));
  }

  protected override void Save(object node) {
    Adapter.ConfigNode_Set(node, "amount", this.amount.ToString());
    Adapter.ConfigNode_Set(node, "volume", this.volume.ToString());
  }

  public override void OnActivate(VirtualVessel virtualVessel) {
    throw new NotImplementedException();
  }
}
