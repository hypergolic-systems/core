using System;
using Hgs.Core.Resources;
using Hgs.Core.Virtual;

namespace Hgs.Game.Components.Tankage;

public class Tank : VirtualComponent {

  public Resource Resource = null;
  public float Amount = 0;
  public float Volume = 0;

  protected override void Load(ConfigNode node) {
    Amount = float.Parse(node.GetValue("amount"));
    Volume = float.Parse(node.GetValue("volume"));
  }

  protected override void Save(ConfigNode node) {
    node.AddValue("amount", Amount.ToString());
    node.AddValue("volume", Volume.ToString());
  }

  public override void OnActivate(VirtualVessel virtualVessel) {
    throw new NotImplementedException();
  }
}
