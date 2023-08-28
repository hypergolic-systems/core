
using Hgs.Core.System.Electrical.Components;
using Hgs.Core.Virtual;
using Hgs.Test.FakeKSP;

namespace Hgs.Test.FakeMod;
public class FakeRTGModule : FakePartModule, SimulatedModule
{
  public object module => this;

  public object gamePart => this.part;

  public SpacecraftPart spacecraftPart { get; set; }

  public void InitializeComponents(SpacecraftPart part) {
    var rtg = new RadioisotopeThermalGenerator {
      partId = this.part.persistentId,
    };
    part.AddComponent(rtg);
  }

  public void OnLinkToSpacecraft(CompositeSpacecraft sc) {
  }

  public void OnSimulationUpdate(uint delta) {
  }

  public void OnUnlinkFromSpacecraft(CompositeSpacecraft sc) {
  }

  public bool OwnsComponent(VirtualComponent component) {
    return component is RadioisotopeThermalGenerator;
  }
}
