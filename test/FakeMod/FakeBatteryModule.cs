
using Hgs.Core.System.Electrical.Components;
using Hgs.Core.Virtual;
using Hgs.Test.FakeKSP;


namespace Hgs.Test.FakeMod;
public class FakeBatteryModule : FakePartModule, IVirtualPartModule
{
  public object module => this;

  public object gamePart => this.part;

  public VirtualPart virtualPart { get; set; }

  public void InitializeComponents(VirtualVessel virtualVessel, VirtualPart part) {
    var battery = new Battery {
      part = part,
      index = 0,
      Capacity = 100,
    };
    // Fake batteries start empty (more useful for testing).
    part.AddComponent(battery);
  }

  public void OnLinkToSpacecraft(VirtualVessel sc) {
  }

  public void OnSimulationUpdate(uint delta) {
  }

  public void OnSynchronized() {
  }

  public void OnUnlinkFromSpacecraft(VirtualVessel sc) {
  }

  public bool OwnsComponent(VirtualComponent component) {
    return component is Battery;
  }
}
