using System.Collections.Generic;
using Hgs.Core.System.Electrical;
using Hgs.Core.Virtual;

namespace Hgs.Mod.Modules;

public class HgPartBattery : PartModule, VirtualizedModule
{

  public object module { get { return this; } }

  public List<VirtualPart> virtualParts { get; set; }

  [KSPField]
  public int capacity = 0;

  [
    KSPField(guiActive = true, guiActiveEditor = true, isPersistant = false),
    UI_ProgressBar(minValue = 0, maxValue = 1),
  ]
  public float StoredEnergy = 0;

  protected Battery battery
  {
    get
    {
      return this.virtualParts[0] as Battery;
    }
  }

  public void InitializeVirtualParts()
  {
    var battery = new Battery(this.part.persistentId, 0);
    battery.InitializeCapacity(capacity);
    this.virtualParts = new List<VirtualPart>(1);
    this.virtualParts.Add(battery);
  }

  public void OnLinkToSpacecraft(CompositeSpacecraft sc)
  {
    (Fields["StoredEnergy"].uiControlEditor as UI_ProgressBar).maxValue = battery.GetWattsCapacity();
    (Fields["StoredEnergy"].uiControlFlight as UI_ProgressBar).maxValue = battery.GetWattsCapacity();
  }

  public void OnSimulationUpdate(uint delta)
  {
    StoredEnergy = battery.GetWattsStored();
  }

  public void OnUnlinkFromSpacecraft(CompositeSpacecraft sc)
  {
    throw new global::System.NotImplementedException();
  }
}
