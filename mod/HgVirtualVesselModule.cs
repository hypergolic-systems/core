
using Hgs.Core;
using Hgs.Core.Simulation;
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
      virtualVessel = new VirtualVessel();
    }

    public override Activation GetActivation() {
      return Activation.AllScenes;
    }

    protected override void OnStart() {
      base.OnStart();
      UnityEngine.Debug.Log($"[HGS] VirtualVesselModule OnStart for ${vessel?.persistentId} : ${vessel?.vesselName } /{vessel?.GetDisplayName()}");
      virtualVessel.liveVessel = this;
      foreach (var resource in virtualVessel.resources.Values) {
        SimulationDriver.Instance.AddTarget(resource);
      }

      foreach (var part in virtualVessel.virtualParts.Values) {
        foreach (var component in part.components) {
          component.OnActivate(virtualVessel);
        }
      }
    }

    public override void OnLoadVessel() {
      base.OnLoadVessel();

      if (vessel == null || vessel.parts == null) {
        return;
      }

      // VirtualVesselManager.Instance.OnLoadVessel(vessel);
    }

    public override void OnUnloadVessel() {
      VirtualVesselManager.Instance.OnUnloadVessel(vessel);
      base.OnUnloadVessel();
    }

    protected override void OnLoad(ConfigNode node) {
      base.OnLoad(node);
      UnityEngine.Debug.Log("VirtualVesselModule OnLoad");
      virtualVessel?.OnLoad(node);
    }

    protected override void OnSave(ConfigNode node) {
      base.OnSave(node);
      virtualVessel?.OnSave(node);
    }
  }
}
