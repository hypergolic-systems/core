using Hgs.Core.Resources;
using Hgs.Core.Virtual;


namespace Hgs.Game.Components.Electrical;

public class Battery : VirtualComponent, ResourceSystem.IBuffer {

  public float Amount {get; set;} = 0;
  public float Capacity {get; set;} = 0;

  public float Rate { get; set; } = 0;

  protected override void Load(ConfigNode node) {
    Amount = int.Parse(node.GetValue("stored"));
    Capacity = int.Parse(node.GetValue("capacity"));
  }

  protected override void Save(ConfigNode node) {
    node.AddValue("stored", Amount.ToString());
    node.AddValue("capacity", Capacity.ToString());
  }
  public void Commit() {}

  public override void OnActivate(VirtualVessel virtualVessel) {
    virtualVessel.resources[Resource.ElectricCharge].AddBuffer(this);
  }
}
