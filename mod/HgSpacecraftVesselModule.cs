using System;
using System.Collections.Generic;
using Hgs.Core.Virtual;
using Hgs.Core.System.Electrical;
using System.Runtime.InteropServices;

namespace Hgs.Mod.Virtual {

  /**
   * Provides the link between the simulated version of a `Spacecraft` with `SimulatedPart`s
   * and the game version of `Vessel`s.
   */
  public class HgSpacecraftVesselModule : VesselModule {
    /**
     * The `Spacecraft` associated with this vessel, or `null` if none exists.
     */
    public CompositeSpacecraft composite;

    protected override void OnAwake() {
      base.OnAwake();
    }

    public override void OnLoadVessel() {
      base.OnLoadVessel();

      if (vessel == null || vessel.parts == null) {
        return;
      }
      SpacecraftManager.Instance.OnLoadVessel(vessel);
    }

    public override void OnUnloadVessel() {
      SpacecraftManager.Instance.OnUnloadVessel(vessel);
      base.OnUnloadVessel();
    }

    protected override void OnLoad(ConfigNode node) {
      base.OnLoad(node);
      SpacecraftManager.Instance.OnLoadVesselConfig(node);
    }

    protected override void OnSave(ConfigNode node) {
      base.OnSave(node);
    }
  }
}
