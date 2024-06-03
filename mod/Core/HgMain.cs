using Hgs.Core.Simulator;
using UnityEngine;
using Hgs.RemoteUi;
using Hgs.RemoteUi.Rpc;
using System.Linq;
using Hgs.Core.Virtual;

namespace Hgs;

[KSPAddon(KSPAddon.Startup.AllGameScenes, true)]
public class HgMain : MonoBehaviour {

  protected static uint MAX_TIME_DELTA = 3600;

  protected ulong LastUpdateTime = 0;

  protected ulong WorldTime {
    get {
      return (ulong) Planetarium.GetUniversalTime();
    }
  }

  public HgMain() {
    DontDestroyOnLoad(this);
  }

  public void Awake() {
    GameEvents.onGameStatePostLoad.Add(OnGameLoaded);
    GameEvents.onVesselWasModified.Add(OnVesselWasModified);
    LoadPartStaticData();
    SimulationDriver.Initialize();
    RemoteUiServer.Initialize();
    RemoteUiServer.RegisterHandler("/status", (ctx) => new StatusRequest(ctx));
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

  private void LoadPartStaticData() {
    foreach (var part in PartLoader.LoadedPartsList) {
      if (part.partPrefab == null) {
        continue;
      }

      foreach (var module in part.partPrefab.Modules) {
        if (!(module is HgVirtualPartModule)) {
          continue;
        }
        Util.Log($"Attempt static data for {module.moduleName} on {part.name}");
        var vpm = (HgVirtualPartModule) module;
        var moduleConfig = part.partConfig.GetNodes("MODULE").FirstOrDefault(n => n.GetValue("name") == module.moduleName);
        if (moduleConfig == null) {
          Util.Log($"No static config found for {module.moduleName} on {part.name}");
          continue;
        }

        vpm.LoadStaticData(moduleConfig);
      }
    }
  }
}

