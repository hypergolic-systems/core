using System;
using Hgs.Core.Virtual;

namespace Hgs.Game.Components.Tankage;

public class Tank : VirtualComponent {

  public TankedSubstance substance = TankedSubstance.Empty;
  public float amount = 0;
  public float volume = 0;

  public TankedSubstance GetSubstance() {
    return substance;
  }

  protected override void Load(ConfigNode node) {
    amount = float.Parse(node.GetValue("amount"));
    volume = float.Parse(node.GetValue("volume"));
  }

  protected override void Save(ConfigNode node) {
    node.AddValue("amount", amount.ToString());
    node.AddValue("volume", volume.ToString());
  }

  public override void OnActivate(VirtualVessel virtualVessel) {
    throw new NotImplementedException();
  }
}
