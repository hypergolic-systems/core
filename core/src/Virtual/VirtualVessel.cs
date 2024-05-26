using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hgs.Core.Simulation;

namespace Hgs.Core.Virtual;

/// <summary>
/// A virtual analogue to `Vessel` from KSP.
///
/// Virtual vessels are composed of one or more `Spacecraft`, which in turn have one or more
/// `Segment`s (stages). `Part`s in the vessel may (but not necessarily) have a corresponding
/// `VirtualPart`, which is tracked in this tree.
///
/// `VirtualVessel`s exist and are simulated independently of the real `Vessel`, which KSP loads
/// and unloads depending on the game state.
/// </summary>
public class VirtualVessel {

  public List<Spacecraft> spacecraft = new();
  public Dictionary<uint, VirtualPart> virtualParts = new();
  public Dictionary<uint, Segment> segmentsByPart = new();
  public Segment rootSegment;

  public Dictionary<uint, ResourceSystem> resources = new();

  public object liveVessel = null;

  public VirtualVessel() {
    resources.Add(WellKnownResource.Electricity, new ResourceSystem(new ElectricFlowResolver()));
  }

  public void Clear() {
    this.spacecraft.Clear();
    this.segmentsByPart.Clear();
  }

  public void OnSynchronized() {
    foreach (var part in virtualParts.Values) {
      part.virtualModule?.OnSynchronized();
    }
  }

  public bool IsLive {
    get => liveVessel != null && Adapter.Vessel_rootPart(liveVessel) != null;
  }

  public void CreateVirtualPart(object part, IVirtualPartModule module) {
    var partId = Adapter.Part_persistentId(part);
    Debug.Assert(!virtualParts.ContainsKey(partId), "Part already exists in the virtual vessel");
    module.virtualPart = new VirtualPart {
      id = partId,
      spacecraft = spacecraft[0],
      virtualModule = module,
    };
    virtualParts[partId] = module.virtualPart;
  }

  public void OnLoad(object vesselNode) {
    var node = Adapter.ConfigNode_GetNode(vesselNode, "HGS_VIRTUAL_VESSEL");
    if (node == null) {
      return;
    }

    foreach (var scNode in Adapter.ConfigNode_GetNodes(node, "SPACECRAFT")) {
      var sc = new Spacecraft() {
        virtualVessel = this,
        controllingPart = uint.Parse(Adapter.ConfigNode_Get(scNode, "controllingPart")),
      };
      spacecraft.Add(sc);

      foreach (var partNode in Adapter.ConfigNode_GetNodes(scNode, "PART")) {
        var part = new VirtualPart {
          id = uint.Parse(Adapter.ConfigNode_Get(partNode, "id")),
        };
        virtualParts.Add(part.id, part);
        var index = 0;
        foreach (var componentNode in Adapter.ConfigNode_GetNodes(partNode, "COMPONENT")) {
          var cmp = VirtualComponent.FromConfig(part, componentNode, /* initial */ false);
          cmp.index = index++;
          part.AddComponent(cmp);
        }
      }
    }

    if (IsLive) {
      SynchronizeWithKsp();
    }
  }

  public void OnSave(object vesselNode) {
    var node = Adapter.ConfigNode_Create("HGS_VIRTUAL_VESSEL");
    foreach (var sc in spacecraft) {
      var scNode = Adapter.ConfigNode_Create("SPACECRAFT");
      Adapter.ConfigNode_Set(scNode, "controllingPart", sc.controllingPart.ToString());
      foreach (var part in this.virtualParts.Values.Where(p => p.spacecraft == sc)) {
        var partNode = Adapter.ConfigNode_Create("PART");
        Adapter.ConfigNode_Set(partNode, "id", part.id.ToString());
        foreach (var component in part.components) {
          Adapter.ConfigNode_AddNode(partNode, component.SaveConfig(/* initial */false));
        }
        Adapter.ConfigNode_AddNode(scNode, partNode);
      }
      Adapter.ConfigNode_AddNode(node, scNode);
    }
    Adapter.ConfigNode_AddNode(vesselNode, node);
  }

  private void SynchronizeWithKsp() {

  }
}
