using Hgs.Core.Virtual;
using Hgs.Core.Resources;

namespace Hgs.Game.Components.Electrical;

public class RadioisotopeThermalGenerator : VirtualComponent, ResourceSystem.IProducer {
  public int Priority => 0;

  public float BaselineProduction => 10;

  public float DynamicProductionLimit => 0;

  public float DynamicProductionRate { get; set; } = 0;

  public ulong RemainingValidDeltaT => ulong.MaxValue;

  public void Commit() {}

  public override void OnActivate(VirtualVessel virtualVessel) {
    virtualVessel.resources[Resource.ElectricCharge].AddProducer(this);
  }

  public void Tick(ulong deltaT) {}

  protected override void Load(ConfigNode node) {}
  protected override void Save(ConfigNode node) {}
}
