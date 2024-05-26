using System.Linq;
using Hgs.Core.System.Electrical.Components;

namespace Hgs.Mod.Modules;

public class HgPartBattery : HgPartBase {
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
    UnityEngine.Debug.Log($"[HGS] HgPartBattery OnStart in state {state}");
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
    UnityEngine.Debug.Log($"[HGS] {GetType().Name} InitializeComponents");
    virtualPart.AddComponent(new Battery {
      Capacity = capacity,
      Stored = capacity,
    });
  }

  public override void OnSynchronized() {
    StoredEnergy = (float) battery.Stored;
  }
}
