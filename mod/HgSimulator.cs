using System;
using Hgs.Core;
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
    Adapter.Instance = new KspAdapter();
  }

  public void Awake() {
    GameEvents.onGameStatePostLoad.Add(OnGameLoaded);
    GameEvents.onVesselWasModified.Add(OnVesselWasModified);
  }

  public void FixedUpdate() {
    // Preconditions
    if (FlightGlobals.Vessels == null) {
      Debug.LogError("no vessels found");
      return;
    }

    Debug.Log("FixedUpdate: before logic");

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
        var composite = CompositeManager.Instance.GetSpacecraft(vessel);
        if (composite == null) {
          continue;
        }

        try {
          // composite.simulator.Simulate(delta);
        } catch (Exception e) {
          Debug.LogError("Exception while simulating spacecraft: " + e.Message + "\n" + e.StackTrace.ToString());
        }
      }
    }

    LastUpdateTime = CurrentWorldTime;
  }

  public void OnGameLoaded(ConfigNode _) {
    LastUpdateTime = 0;
  }

  public void OnVesselWasModified(Vessel vessel) {
    // TODO: dedicated method maybe?
    // SpacecraftManager.Instance.OnLoadVessel(vessel);
  }
}

