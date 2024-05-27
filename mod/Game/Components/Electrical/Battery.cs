using Hgs.Core.Resources;
using Hgs.Core.Virtual;


namespace Hgs.Game.Components.Electrical;

public class Battery : VirtualComponent {

  public double Stored = 0;
  public double Capacity = 0;

  public ResourceFlow flow;

  protected override void Load(ConfigNode node) {
    Stored = int.Parse(node.GetValue("stored"));
    Capacity = int.Parse(node.GetValue("capacity"));
  }

  protected override void Save(ConfigNode node) {
    node.AddValue("stored", Stored.ToString());
    node.AddValue("capacity", Capacity.ToString());
  }

  public override void OnActivate(VirtualVessel virtualVessel) {
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

