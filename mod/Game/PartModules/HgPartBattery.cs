using System.Linq;
using Hgs.Core.Virtual;
using Hgs.Game.Components.Electrical;
using UnityEngine;

namespace Hgs.Game.PartModules;

public class HgPartBattery : HgVirtualPartModule {
  [KSPField]
  public int capacity = 0;

  [
    KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false),
    UI_ProgressBar(minValue = 0, maxValue = 1),
  ]
  public float StoredEnergy = 0;

  public Battery battery = null;

  public override void OnAwake() {
    base.OnAwake();
  }

  public override void OnStart(StartState state) {
    base.OnStart(state);
    this.battery = this.virtualPart.components.OfType<Battery>().First();
    (Fields["StoredEnergy"].uiControlEditor as UI_ProgressBar).maxValue = (float) capacity;
    (Fields["StoredEnergy"].uiControlFlight as UI_ProgressBar).maxValue = (float) capacity;
    if (IsInEditor) {
      StoredEnergy = (float) battery.Stored;
      Fields["StoredEnergy"].OnValueModified += (_) => {
        battery.Stored = (double) StoredEnergy;
      };
    }
  }

  protected override void InitializeComponents() {
    virtualPart.AddComponent(new Battery {
      Capacity = capacity,
      Stored = capacity,
    });
  }

  public override void OnSynchronized() {
    base.OnSynchronized();

    StoredEnergy = (float) battery.Stored;
  }
}
