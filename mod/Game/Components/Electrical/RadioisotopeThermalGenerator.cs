using Hgs.Core.Virtual;
using Hgs.Core.Resources;

namespace Hgs.Game.Components.Electrical;

public class RadioisotopeThermalGenerator : VirtualComponent {

  public ResourceFlow flow;

  public override void OnActivate(VirtualVessel virtualVessel) {
    this.flow = virtualVessel.resources[Resource.ElectricCharge].NewFlow();
    this.flow.Name = $"RTG({part.id})";
    this.flow.CanProduceRate = 10;
    this.flow.Priority = 0;
    this.flow.StorageTier = 10;
  }

  protected override void Load(ConfigNode node) {}
  protected override void Save(ConfigNode node) {}
}
