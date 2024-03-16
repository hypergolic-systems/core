using System;
using Hgs.Core.Virtual;
using Hgs.Core.System.Electrical;
using Hgs.Core.Simulation;

namespace Hgs.Core.System.Electrical.Components;

public class Battery : VirtualComponent {

  public double Stored = 0;
  public double Capacity = 0;

  public ResourceFlow flow;

  public override void Load(object node) {
    base.Load(node);
    Stored = int.Parse(Adapter.ConfigNode_Get(node, "stored"));
    Capacity = int.Parse(Adapter.ConfigNode_Get(node, "capacity"));
    flow.ActiveRate = 0;
    flow.CanProduceRate = 0;
    flow.CanConsumeRate = 10;
  }

  public override void Save(object node) {
    base.Save(node);
    Adapter.ConfigNode_Set(node, "stored", Stored.ToString());
    Adapter.ConfigNode_Set(node, "capacity", Capacity.ToString());
  }

  public override void OnAttached(Composite composite) {
    base.OnAttached(composite);
    this.flow = composite.resources[WellKnownResource.Electricity].NewFlow();
    this.flow.Name = $"Battery({partId})";
    this.flow.CanProduceRate = Stored > 0 ? 10 : 0;
    this.flow.CanConsumeRate = Stored < Capacity ? 10 : 0;
    this.flow.StorageTier = 5;
    this.flow.OnFlow = OnFlow;
    this.flow.OnSetActiveRate = OnSetActiveRate;
  }

  public void InitializeCapacity(int watts) {
    Stored = 0;
    Capacity = watts;
  }

  public void OnFlow(double amount) {
    Stored += amount;
    flow.CanProduceRate = Stored > 0 ? 10 : 0;
    flow.CanConsumeRate = Stored < Capacity ? 10 : 0;
    this.calculateRemainingValidDeltaT(flow.ActiveRate);
  }

  public void OnSetActiveRate(double rate) {
    this.calculateRemainingValidDeltaT(rate);
  }

  private void calculateRemainingValidDeltaT(double rate) {
    if (rate > 0) {
      flow.RemainingValidDeltaT = (Capacity - Stored) / rate;
    } else {
      flow.RemainingValidDeltaT = double.MaxValue;
    }
  }
}

