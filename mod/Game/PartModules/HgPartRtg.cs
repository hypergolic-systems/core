using Hgs.Game.Components.Electrical;
using Hgs.Core.Virtual;

namespace Hgs.Game.PartModules;

public class HgPartRtg : HgVirtualPartModule {
  [KSPField]
  public int power = 0;

  public override void InitializeComponents() {
    VirtualPart.AddComponent(new RadioisotopeThermalGenerator {});
  }
}