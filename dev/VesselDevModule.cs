namespace Hgs.Dev;

public class VesselDevModule : VesselModule {

  public VesselDevModule() {
    UnityEngine.Debug.Log("VesselDevModule instantiated");
  }

  public override void OnLoadVessel() {
    base.OnLoadVessel();
    UnityEngine.Debug.Log("OnLoadVessel: " + this.vessel.persistentId + " " + this.vessel.name + ", parts: " + this.vessel.parts.Count);
  }

  public override void OnUnloadVessel() {
    base.OnUnloadVessel();
    UnityEngine.Debug.Log("OnUnoadVessel: " + this.vessel?.persistentId + " " + this.vessel?.name);
  }
}
