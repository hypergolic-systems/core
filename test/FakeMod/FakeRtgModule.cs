
using Hgs.Core.System.Electrical.Components;
using Hgs.Core.Virtual;
using Hgs.Test.FakeKSP;

namespace Hgs.Test.FakeMod;
public class FakeRTGModule : FakePartModule, IVirtualPartModule
{
  public object module => this;

  public object gamePart => this.part;

  public VirtualPart virtualPart { get; set; }

  public void InitializeComponents(VirtualVessel virtualVessel, VirtualPart part) {
    var rtg = new RadioisotopeThermalGenerator {
      part = part,
      index = 0,
    };
    part.AddComponent(rtg);
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
    return component is RadioisotopeThermalGenerator;
  }
}
