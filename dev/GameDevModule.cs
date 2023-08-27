using UnityEngine;

namespace Hgs.Dev;

[KSPAddon (KSPAddon.Startup.MainMenu, true)]
public class GameDevModule : MonoBehaviour {
  public GameDevModule() {
    DontDestroyOnLoad(this);
  }

  public void Start() {
    GameEvents.onVesselWasModified.Add(OnVesselWasModified);
  }

  public void OnVesselWasModified(Vessel vessel) {
    Debug.Log("OnVesselWasModified: " + vessel.persistentId + " " + vessel.name + ", parts: " + vessel.parts.Count);
  }
}
