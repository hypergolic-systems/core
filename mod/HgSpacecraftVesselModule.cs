
using System.Diagnostics;
using Hgs.Core.Virtual;

namespace Hgs.Mod.Virtual {

  /**
   * Provides the link between the simulated version of a `Spacecraft` with `SimulatedPart`s
   * and the game version of `Vessel`s.
   */
  public class HgSpacecraftVesselModule : VesselModule {
    /**
     * The `Composite` associated with this vessel, or `null` if none exists.
     */
    public Composite composite;

    protected override void OnAwake() {
      base.OnAwake();
    }

    public override void OnLoadVessel() {
      base.OnLoadVessel();

      if (vessel == null || vessel.parts == null) {
        return;
      }

      CompositeManager.Instance.OnLoadVessel(vessel);
    }

    public override void OnUnloadVessel() {
      CompositeManager.Instance.OnUnloadVessel(vessel);
      base.OnUnloadVessel();
    }

    protected override void OnLoad(ConfigNode node) {
      base.OnLoad(node);
      CompositeManager.Instance.OnLoadVesselConfig(node);
    }

    protected override void OnSave(ConfigNode node) {
      base.OnSave(node);
    }
  }
}
