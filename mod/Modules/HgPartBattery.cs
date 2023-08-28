using System;
using System.Collections.Generic;
using System.Linq;
using Hgs.Core.System.Electrical.Components;
using Hgs.Core.Virtual;

namespace Hgs.Mod.Modules;

public class HgPartBattery : HgPartBase {
  [KSPField]
  public int capacity = 0;

  [
    KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false),
    UI_ProgressBar(minValue = 0, maxValue = 1),
  ]
  public float StoredEnergy = 0;

  public Battery battery;

  public override void InitializeComponents(SpacecraftPart part) {
    var battery = new Battery {
      partId = this.part.persistentId,
    };
    battery.InitializeCapacity(capacity);
    part.AddComponent(battery);
  }

  public override void OnLinkToSpacecraft(CompositeSpacecraft sc) {
    this.battery = this.spacecraftPart.components.OfType<Battery>().First();
    (Fields["StoredEnergy"].uiControlEditor as UI_ProgressBar).maxValue = battery.Capacity;
    (Fields["StoredEnergy"].uiControlFlight as UI_ProgressBar).maxValue = battery.Capacity;
  }

  public override void OnSimulationUpdate(uint delta) {
    StoredEnergy = battery.Stored;
  }

  public override void OnUnlinkFromSpacecraft(CompositeSpacecraft sc) {
  }

  public override bool OwnsComponent(VirtualComponent component) {
    return component is Battery;
  }
}
