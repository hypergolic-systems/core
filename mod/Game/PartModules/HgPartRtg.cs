using Hgs.Game.Components.Electrical;
using Hgs.Core.Virtual;

namespace Hgs.Game.PartModules;

public class HgPartRtg : HgVirtualPartModule {
  [KSPField]
  public int power = 0;

  protected override void InitializeComponents() {
    virtualPart.AddComponent(new RadioisotopeThermalGenerator {});
  }
}
