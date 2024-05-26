using Hgs.Core.Virtual;
using Hgs.Core.Simulation;

namespace Hgs.Core.System.Electrical.Components;

public class RadioisotopeThermalGenerator : VirtualComponent {

  public ResourceFlow flow;

  public override void OnActivate(VirtualVessel virtualVessel) {
    this.flow = virtualVessel.resources[WellKnownResource.Electricity].NewFlow();
    this.flow.Name = $"RTG({part.id})";
    this.flow.CanProduceRate = 10;
    this.flow.Priority = 0;
    this.flow.StorageTier = 10;
  }

  protected override void Load(object node) {}
  protected override void Save(object node) {}
}
