
using Hgs.Core.System.Electrical.Components;
using Hgs.Core.Virtual;
using Hgs.Test.FakeKSP;

namespace Hgs.Test.FakeMod;
public class FakeRTGModule : FakePartModule, VirtualModule
{
  public object module => this;

  public object gamePart => this.part;

  public VirtualPart virtualPart { get; set; }

  public void InitializeComponents(Composite composite, VirtualPart part) {
    var rtg = new RadioisotopeThermalGenerator {
      partId = this.part.persistentId,
    };
    part.AddComponent(rtg);
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
    return component is RadioisotopeThermalGenerator;
  }
}
