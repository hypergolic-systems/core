using Hgs.Core.Virtual;
using Hgs.Core.Simulation;

namespace Hgs.Core.System.Electrical.Components;

public class Battery : VirtualComponent {

  public double Stored = 0;
  public double Capacity = 0;

  public ResourceFlow flow;

  protected override void Load(object node) {
    Stored = int.Parse(Adapter.ConfigNode_Get(node, "stored"));
    Capacity = int.Parse(Adapter.ConfigNode_Get(node, "capacity"));
  }

  protected override void Save(object node) {
    Adapter.ConfigNode_Set(node, "stored", Stored.ToString());
    Adapter.ConfigNode_Set(node, "capacity", Capacity.ToString());
  }

  public override void OnActivate(VirtualVessel virtualVessel) {
    Adapter.Log("Battery.OnActivate");
    this.flow = virtualVessel.resources[WellKnownResource.Electricity].NewFlow();
    this.flow.Name = $"Battery({part.id})";
    this.flow.CanProduceRate = Stored > 0 ? 10 : 0;
    this.flow.CanConsumeRate = Stored < Capacity ? 10 : 0;
    this.flow.StorageTier = 5;
    this.flow.OnFlow = OnFlow;
    this.flow.OnSetActiveRate = OnSetActiveRate;
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

