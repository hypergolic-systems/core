using System;
using Hgs.Core;
using Hgs.Core.Simulation;
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
    SimulationDriver.Initialize();
  }

  public void FixedUpdate() {
    // Preconditions
    if (FlightGlobals.Vessels == null) {
      return;
    }

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

    LastUpdateTime = CurrentWorldTime;
    SimulationDriver.Instance.RaiseUpperBoundOfTime(LastUpdateTime);
    SimulationDriver.Instance.Sync();
  }

  public void OnGameLoaded(ConfigNode _) {
    LastUpdateTime = 0;
  }

  public void OnVesselWasModified(Vessel vessel) {
    // TODO: dedicated method maybe?
    // SpacecraftManager.Instance.OnLoadVessel(vessel);
  }
}

