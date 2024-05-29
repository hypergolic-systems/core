using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Hgs.Core.Resources;
using Hgs.Core.Resources.Resolvers;

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

  public Vessel liveVessel = null;

  public VirtualVessel() {
    resources.Add(WellKnownResource.Electricity, new ResourceSystem(new ElectricFlowResolver()));
  }

  public void Clear() {
    this.spacecraft.Clear();
    this.segmentsByPart.Clear();
  }

  public void OnSynchronized() {
    foreach (var part in virtualParts.Values) {
      part.liveModule?.OnSynchronized();
    }
  }

  public bool IsLive {
    get => liveVessel != null && liveVessel.rootPart != null;
  }

  public void OnLoad(ConfigNode vesselNode) {
    foreach (var partNode in vesselNode.GetNodes("VIRTUAL_PART")) {
      var part = new VirtualPart {
        id = uint.Parse(partNode.GetValue("id")),
      };
      virtualParts.Add(part.id, part);
      var index = 0;
      foreach (var componentNode in partNode.GetNodes("COMPONENT")) {
        var cmp = VirtualComponent.FromConfig(part, componentNode, /* initial */ false);
        cmp.index = index++;
        part.AddComponent(cmp);
      }
    }

    foreach (var scNode in vesselNode.GetNodes("SPACECRAFT")) {
      var sc = new Spacecraft() {
        virtualVessel = this,
        controllingPart = uint.Parse(scNode.GetValue("controllingPart")),
      };
      spacecraft.Add(sc);
    }

    if (IsLive) {
      SynchronizeWithKsp();
    }
  }

  public void OnSave(ConfigNode vesselNode) {
    foreach (var part in virtualParts.Values) {
      var partNode = vesselNode.AddNode("VIRTUAL_PART");
      partNode.AddValue("id", part.id.ToString());
      foreach (var component in part.Components) {
        component.SaveConfig(partNode.AddNode("COMPONENT"), /* initial */false);
      }
    }
    foreach (var sc in spacecraft) {
      var scNode = vesselNode.AddNode("SPACECRAFT");
      scNode.AddValue("controllingPart", sc.controllingPart.ToString());
    }
  }

  private void SynchronizeWithKsp() {

  }
}
