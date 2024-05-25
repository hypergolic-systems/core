using System.Collections.Generic;
using System.Linq;
using Hgs.Core.Simulation;

namespace Hgs.Core.Virtual;

public class VirtualVessel {
  public uint id;

  public bool IsDirty { get; set; } = true;

  public List<Spacecraft> spacecraft = new();
  public Dictionary<uint, VirtualPart> partMap = new();
  public Dictionary<uint, Segment> segmentsByPart = new();
  public Segment rootSegment;

  public Dictionary<uint, ResourceSystem> resources = new();

  public object liveVessel = null;

  public VirtualVessel(uint vesselId) {
    this.id = vesselId;

    resources.Add(WellKnownResource.Electricity, new ResourceSystem(new ElectricFlowResolver()));
  }

  public void Clear() {
    this.spacecraft.Clear();
    this.segmentsByPart.Clear();
  }

  public void OnSynchronized() {
    if (this.liveVessel == null) {
      return;
    }
    // TODO: we almost certainly need to cache this for performance
    var virtualModules = Adapter.Vessel_FindPartModulesImplementing<VirtualModule>(this.liveVessel);
    foreach (var module in virtualModules) {
      module.OnSynchronized();
    }
  }
}
