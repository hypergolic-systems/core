using System;
using Hgs.Core.Virtual;
using UnityEngine;

namespace Hgs.Mod.Virtual;
[KSPAddon(KSPAddon.Startup.AllGameScenes, true)]
public class HgSimulator : MonoBehaviour {

  protected static uint MAX_TIME_DELTA = 3600;

  protected ulong LastUpdateTime = 0;

  protected ulong WorldTime {
    get {
      return (ulong) Planetarium.GetUniversalTime();
    }
  }

  public HgSimulator() {
    DontDestroyOnLoad(this);
  }

  public void Awake() {
    GameEvents.onGameStatePostLoad.Add(OnGameLoaded);
    GameEvents.onVesselWasModified.Add(OnVesselWasModified);
  }

  public void FixedUpdate() {
    var CurrentWorldTime = WorldTime;
    if (LastUpdateTime == 0) {
      // Guard against accidentally doing a giant update from time 0.
      LastUpdateTime = CurrentWorldTime;
      return;
    }

    var totalDelta = (uint)(CurrentWorldTime - LastUpdateTime);
    if (totalDelta == 0) {
      // No update needed.
      return;
    }

    while (totalDelta > 0) {
      var delta = Math.Min(totalDelta, MAX_TIME_DELTA); 
      totalDelta -= delta;

      // TODO: keep a cached list of vessels
      foreach (var vessel in FlightGlobals.Vessels) {
        var composite = SpacecraftManager.Instance.GetSpacecraft(vessel);
        if (composite == null) {
          continue;
        }

        composite.simulator.Simulate(delta);
      }
    }

    LastUpdateTime = CurrentWorldTime;
  }

  public void OnGameLoaded(ConfigNode _) {
    LastUpdateTime = 0;
  }

  public void OnVesselWasModified(Vessel vessel) {
    // TODO: dedicated method maybe?
    SpacecraftManager.Instance.OnLoadVessel(vessel);
  }
}

