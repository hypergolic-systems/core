using Hgs.Core.Virtual;
using Hgs.Core.Simulation;

namespace Hgs.Core.System.Electrical.Components;

public class RadioisotopeThermalGenerator : VirtualComponent {

  public ResourceFlow flow;

  public override void OnAttached(VirtualVessel virtualVessel) {
    base.OnAttached(virtualVessel);
    this.flow = virtualVessel.resources[WellKnownResource.Electricity].NewFlow();
    this.flow.Name = $"RTG({partId})";
    this.flow.CanProduceRate = 10;
    this.flow.Priority = 0;
    this.flow.StorageTier = 10;
  }
}
