using Hgs.Core.System.Electrical.Components;
using Hgs.Core.Virtual;

namespace Hgs.Mod.Modules;

public class HgPartRtg : HgPartBase {
  [KSPField]
  public int power = 0;

  public override void InitializeComponents(VirtualVessel sc, VirtualPart part) {
    var rtg = new RadioisotopeThermalGenerator {
      partId = this.part.persistentId,
      // WattsPerSecond = power,
    };
    part.AddComponent(rtg);
  }

  public override bool OwnsComponent(VirtualComponent component) {
    return component is RadioisotopeThermalGenerator;
  }
}
