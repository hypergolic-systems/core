using Hgs.Core.System.Electrical.Components;
using Hgs.Core.Virtual;

namespace Hgs.Mod.Modules;

public class HgPartRtg : HgPartBase {
  [KSPField]
  public int power = 0;

  public override void InitializeComponents(SpacecraftPart part) {
    var rtg = new RadioisotopeThermalGenerator {
      partId = this.part.persistentId,
      WattsPerSecond = power,
    };
    part.AddComponent(rtg);
  }

  public override void OnLinkToSpacecraft(CompositeSpacecraft sc) {
  }

  public override void OnSimulationUpdate(uint delta) {
  }

  public override void OnUnlinkFromSpacecraft(CompositeSpacecraft sc) {
  }

  public override bool OwnsComponent(VirtualComponent component) {
    return component is RadioisotopeThermalGenerator;
  }
}
