using System;
using UnityEngine;

namespace Hgs.Virtual {
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
          var spacecraftModule = vessel.GetComponent<HgSpacecraftVesselModule>();
          if (spacecraftModule == null || spacecraftModule.craft == null) {
            continue;
          }

          spacecraftModule.craft.Tick(delta);
        }
      }

      LastUpdateTime = CurrentWorldTime;
    }

    public void OnGameLoaded(ConfigNode _) {
      LastUpdateTime = 0;
    }
  }
}
