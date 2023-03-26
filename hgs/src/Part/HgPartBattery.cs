using Hgs.System;
using Hgs.System.Electrical;
using KSP.UI.Screens.DebugToolbar.Screens.Debug;
using UnityEngine;

namespace Hgs.Part {

  public class HgPartBattery : HgSimulatedPartModule {

    [KSPField]
    public int capacity = 0;

    [
      KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false),
      UI_ProgressBar(minValue = 0, maxValue = 1),
    ]
    public float StoredEnergy = 0;

    protected Battery battery {
      get {
        return this.simPart as Battery;
      }
    }

    public override SimulatedPart CreateSimulatedPart() {
      var battery = new Battery(this.part.persistentId);
      battery.InitializeCapacity(capacity);
      return battery;
    }

    public override void OnLinkToSpacecraft(Spacecraft sc) {
      (Fields["StoredEnergy"].uiControlEditor as UI_ProgressBar).maxValue = battery.GetWattsCapacity();
      (Fields["StoredEnergy"].uiControlFlight as UI_ProgressBar).maxValue = battery.GetWattsCapacity();
    }

    public override void OnSimulationUpdate(uint delta) {
      StoredEnergy = battery.GetWattsStored();
    }
  }
}
