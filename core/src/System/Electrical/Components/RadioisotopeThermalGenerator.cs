using System;
using Hgs.Core.Virtual;
using Hgs.Core.System.Electrical;
using Hgs.Core.Simulation;

namespace Hgs.Core.System.Electrical.Components;

public class RadioisotopeThermalGenerator : VirtualComponent {

  private ResourceFlow flow;

  public override void OnAttached(Composite composite) {
    base.OnAttached(composite);
    this.flow = composite.resources[WellKnownResource.Electricity].NewFlow();
    this.flow.Name = $"RTG({partId})";
    this.flow.CanProduceRate = 10;
    this.flow.Priority = 0;
    this.flow.StorageTier = 10;
  }
}
