using System.Collections.Generic;
using Hgs.Virtual;
using Hgs.System.Electrical;

namespace Hgs.Part {

  public class HgPartBattery : PartModule, VirtualizedModule {

    public PartModule module { get { return this; } }

    public List<VirtualPart> virtualParts { get; set; }

    [KSPField]
    public int capacity = 0;

    [
      KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false),
      UI_ProgressBar(minValue = 0, maxValue = 1),
    ]
    public float StoredEnergy = 0;

    protected Battery battery {
      get {
        return this.virtualParts[0] as Battery;
      }
    }

    public void InitializeVirtualParts() {
      var battery = new Battery(this.part.persistentId, 0);
      battery.InitializeCapacity(capacity);
      this.virtualParts = new List<VirtualPart>(1);
      this.virtualParts.Add(battery);
    }

    public void OnLinkToSpacecraft(VirtualVessel sc) {
      (Fields["StoredEnergy"].uiControlEditor as UI_ProgressBar).maxValue = battery.GetWattsCapacity();
      (Fields["StoredEnergy"].uiControlFlight as UI_ProgressBar).maxValue = battery.GetWattsCapacity();
    }

    public void OnSimulationUpdate(uint delta) {
      StoredEnergy = battery.GetWattsStored();
    }

    void VirtualizedModule.OnUnlinkFromSpacecraft(VirtualVessel sc) {
      throw new global::System.NotImplementedException();
    }
  }
}
