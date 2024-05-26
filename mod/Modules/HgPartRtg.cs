using Hgs.Core.System.Electrical.Components;
using Hgs.Core.Virtual;

namespace Hgs.Mod.Modules;

public class HgPartRtg : HgPartBase {
  [KSPField]
  public int power = 0;

  protected override void InitializeComponents() {
    virtualPart.AddComponent(new RadioisotopeThermalGenerator {});
  }
}
