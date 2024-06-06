using Hgs.Core.Resources;
using Hgs.Core.Virtual;

namespace Hgs.Game.Components.Tankage;

public class Tank : VirtualComponent, ResourceSystem.IBuffer {

  public Resource Resource = null;
  public float Amount {get; set;} = 0;
  public float Capacity {get; set;} = 0;
  public float Rate { get; set; } = 0;

  protected override void Load(ConfigNode node) {
    Amount = float.Parse(node.GetValue("amount"));
    Capacity = float.Parse(node.GetValue("volume"));
  }

  protected override void Save(ConfigNode node) {
    node.AddValue("amount", Amount.ToString());
    node.AddValue("volume", Capacity.ToString());
  }

  public override void OnActivate(VirtualVessel virtualVessel) {
    virtualVessel.resources[Resource].AddBuffer(this);
  }

  public void Commit() {}
}
