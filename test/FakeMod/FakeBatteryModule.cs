
using Hgs.Core.System.Electrical.Components;
using Hgs.Core.Virtual;
using Hgs.Test.FakeKSP;

namespace Hgs.Test.FakeMod;
public class FakeBatteryModule : FakePartModule, SimulatedModule
{
  public object module => this;

  public object gamePart => this.part;

  public SpacecraftPart spacecraftPart { get; set; }

  public void InitializeComponents(SpacecraftPart part) {
    var battery = new Battery {
      partId = this.part.persistentId,
    };
    battery.InitializeCapacity(100);
    // Fake batteries start empty (more useful for testing).
    battery.Stored = 0;
    part.AddComponent(battery);
  }

  public void OnLinkToSpacecraft(CompositeSpacecraft sc) {
  }

  public void OnSimulationUpdate(uint delta) {
  }

  public void OnUnlinkFromSpacecraft(CompositeSpacecraft sc) {
  }

  public bool OwnsComponent(VirtualComponent component) {
    return component is Battery;
  }
}
