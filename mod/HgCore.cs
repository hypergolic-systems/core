using System;
using Hgs.Core;
using Hgs.Core.Simulation;
using UnityEngine;
using Hgs.Core.RemoteUi;
using Hgs.Mod.Rpc;

namespace Hgs.Mod.Virtual;
[KSPAddon(KSPAddon.Startup.AllGameScenes, true)]
public class HgCore : MonoBehaviour {

  protected static uint MAX_TIME_DELTA = 3600;

  protected ulong LastUpdateTime = 0;

  protected ulong WorldTime {
    get {
      return (ulong) Planetarium.GetUniversalTime();
    }
  }

  public HgCore() {
    DontDestroyOnLoad(this);
    Adapter.Instance = new KspAdapter();
  }

  public void Awake() {
    GameEvents.onGameStatePostLoad.Add(OnGameLoaded);
    GameEvents.onVesselWasModified.Add(OnVesselWasModified);
    SimulationDriver.Initialize();
    RemoteUiServer.Initialize();
    RemoteUiServer.RegisterHandler("/status", (ctx) => new StatusRequest(ctx));
    GameEvents.onVesselUnloaded.Add((vessel) => {
      Debug.Log($"Vessel {vessel.persistentId} / ${vessel.GetDisplayName()} unload event");
    });
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
    if (totalDelta != 0) {
      LastUpdateTime = CurrentWorldTime;
      SimulationDriver.Instance.Sync(LastUpdateTime);
    }

    while (RemoteUiServer.Instance.Requests.TryDequeue(out var request)) {
      request.Run();
    }
  }

  public void OnGameLoaded(ConfigNode _) {
    LastUpdateTime = 0;
  }

  public void OnVesselWasModified(Vessel vessel) {
    // TODO: dedicated method maybe?
    // SpacecraftManager.Instance.OnLoadVessel(vessel);
  }
}

