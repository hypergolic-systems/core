
using Hgs.Core.System.Electrical.Components;
using Hgs.Core.Virtual;
using Hgs.Test.FakeKSP;

namespace Hgs.Test.FakeMod;
public class FakeBatteryModule : FakePartModule, VirtualModule
{
  public object module => this;

  public object gamePart => this.part;

  public VirtualPart virtualPart { get; set; }

  public void InitializeComponents(Composite composite, VirtualPart part) {
    var battery = new Battery {
      partId = this.part.persistentId,
    };
    battery.InitializeCapacity(100);
    // Fake batteries start empty (more useful for testing).
    part.AddComponent(battery);
  }

  public void OnLinkToSpacecraft(Composite sc) {
  }

  public void OnSimulationUpdate(uint delta) {
  }

  public void OnSynchronized() {
  }

  public void OnUnlinkFromSpacecraft(Composite sc) {
  }

  public bool OwnsComponent(VirtualComponent component) {
    return component is Battery;
  }
}
