
using Hgs.Core.Virtual;

namespace Hgs.Mod.Virtual {

  /**
   * Provides the link between a `Vessel` and its corresponding `VirtualVessel`
   * and the game version of `Vessel`s.
   */
  public class HgVirtualVesselModule : VesselModule {
    /**
     * The `VirtualVessel` associated with this vessel, or `null` if none exists.
     */
    public VirtualVessel virtualVessel;

    protected override void OnAwake() {
      base.OnAwake();
    }

    public override void OnLoadVessel() {
      base.OnLoadVessel();

      if (vessel == null || vessel.parts == null) {
        return;
      }

      VirtualVesselManager.Instance.OnLoadVessel(vessel);
    }

    public override void OnUnloadVessel() {
      VirtualVesselManager.Instance.OnUnloadVessel(vessel);
      base.OnUnloadVessel();
    }

    protected override void OnLoad(ConfigNode node) {
      base.OnLoad(node);
      VirtualVesselManager.Instance.OnLoadVesselConfig(node);
    }

    protected override void OnSave(ConfigNode node) {
      base.OnSave(node);
    }
  }
}
