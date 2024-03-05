using Hgs.Core.System.Electrical.Components;
using Hgs.Core.Virtual;

namespace Hgs.Mod.Modules;

public class HgPartRtg : HgPartBase {
  [KSPField]
  public int power = 0;

  public override void InitializeComponents(Composite sc, VirtualPart part) {
    var rtg = new RadioisotopeThermalGenerator {
      partId = this.part.persistentId,
      // WattsPerSecond = power,
    };
    part.AddComponent(rtg);
  }

  public override void OnLinkToSpacecraft(Composite sc) {
  }

  public override void OnSynchronized() {
  }

  public override void OnUnlinkFromSpacecraft(Composite sc) {
  }

  public override bool OwnsComponent(VirtualComponent component) {
    return component is RadioisotopeThermalGenerator;
  }
}
