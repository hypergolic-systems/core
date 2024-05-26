using System;
using System.Collections.Generic;
using System.Linq;
using Hgs.Core.Simulation;

namespace Hgs.Core.Virtual;

/// <summary>
/// Manages `VirtualVessel` instances, which are the virtual representations of physical
/// vessels.
/// </summary>
public class VirtualVesselManager {

  protected Dictionary<uint, VirtualVessel> virtualVessels = new();

  public static VirtualVesselManager Instance = new VirtualVesselManager();

  public void OnDestroyVessel(object vessel) {
    var id = Adapter.Vessel_persistentId(vessel);
    Adapter.Log($"Destroying vessel {id}");
    if (!virtualVessels.ContainsKey(id)) {
      return;
    }

    virtualVessels.Remove(id);

    foreach (var resource in virtualVessels[id].resources.Values) {
      SimulationDriver.Instance.RemoveTarget(resource);
    }
  }

  public void OnUnloadVessel(object vessel) {
    var id = Adapter.Vessel_persistentId(vessel);
    if (!virtualVessels.ContainsKey(id)) {
      return;
    }

    var liveVessel = virtualVessels[id];
    foreach (var part in liveVessel.virtualParts.Values) {
      // Disconnect the VirtualPart from the 
      part.virtualModule = null;
    }
  }

  /// <summary>
  /// Extracts the structure of a `Composite` from the live parts.
  /// </summary>
  protected void LoadStructureFromVessel(VirtualVessel composite, object vessel) {
    composite.Clear();
    var rootCraft = new Spacecraft();
    composite.spacecraft.Add(rootCraft);
  
    var rootPart = Adapter.Vessel_rootPart(vessel);
    var rootSegment = new Segment(rootCraft, Adapter.Part_persistentId(rootPart));
    rootCraft.segmentsByDefiningPart.Add(rootSegment.definingPart, rootSegment);

    this.IngestPartIntoSegmentTree(composite, rootCraft, rootSegment, rootPart, rootPart);
  }

  /// <summary>
  /// Add `part` (including all of its descendents) to the `Composite` tree, creating new
  /// spacecraft or segments if needed.
  /// </summary>
  protected void IngestPartIntoSegmentTree(VirtualVessel composite, Spacecraft craft, Segment segment, object part, object fromPart) {
    var segmentForThisPart = segment;
    var segmentForChildParts = segment;

    // Does this part potentially create a new segment?
    var divider = Adapter.Part_FindModuleImplementing<SegmentDivider>(part);
    if (divider != null) {
      ResolveSegmentDivider(craft, ref segmentForThisPart, ref segmentForChildParts, divider, part, fromPart);
    }

    // Record that this part belongs to the current segment.
    composite.segmentsByPart.Add(Adapter.Part_persistentId(part), segmentForThisPart);

    // Process outgoing edges from this part to others. We actually don't know
    // how we arrived at `part` (either from parent -> child or from child -> parent).
    // So we need to consider both up and down the tree.

    // Child -> Parent
    var parent = Adapter.Part_parent(part);
    if (parent != null && parent != fromPart) {
      this.IngestPartIntoSegmentTree(composite, craft, segmentForChildParts, parent, part);
    }

    // Parent -> Children
    foreach (var childPart in Adapter.Part_children(part)) {
      if (childPart == fromPart) {
        continue;
      }
      this.IngestPartIntoSegmentTree(composite, craft, segmentForChildParts, childPart, part);
    }
  }

  /// <summary>
  /// Determine and apply the effect of a `SegmentDivider` on the current part, creating a new segment if necessary.
  /// </summary>
  protected void ResolveSegmentDivider(Spacecraft composite, ref Segment segmentForThisPart, ref Segment segmentForChildParts, SegmentDivider divider, object part, object fromPart) {
    // Four cases are possible:
    // 1. This part would create a new segment, except it's not coupled to anything.
    // 2. This part is the first of the new segment that was created by `fromPart` (also a separator).
    // 3. This part is the last of the current segment, and couples to a new segment.
    // 4. This part is the first of a new segment not yet created.

    var foreignPart = divider.ForeignPart;
    var thisPartId = Adapter.Part_persistentId(part);
    if (foreignPart == null) {
      // Case 1: no foreign part which would form a new segment.
      // For example, an empty decoupler or docking port.
      //
      // This is a no-op.
    } else if (foreignPart == fromPart && segmentForChildParts.definingPart == thisPartId) {
      // Case 2: this part is the first of the new segment that was created by a paired divider at
      // `fromPart`.
      // For example, the foreign side of a pair of docking ports.
      //
      // This is a no-op.
    } else if (foreignPart != fromPart) {
      // Case 3: we're about to cross the divider, so we're still in the current segment. However,
      // across the divider is a new segment, defined by its first part.
      // For example, a decoupler which will stay attached to `fromPart` when it's decoupled.
      segmentForChildParts = new Segment(composite, Adapter.Part_persistentId(foreignPart));
      composite.segmentsByDefiningPart.Add(segmentForChildParts.definingPart, segmentForChildParts);
    } else {
      // Case 4: this part is the first of a new segment.
      // For example, a decoupler which will detach from `fromPart` when it's decoupled.
      segmentForThisPart = segmentForChildParts = new Segment(composite, thisPartId);
      composite.segmentsByDefiningPart.Add(segmentForChildParts.definingPart, segmentForChildParts);
    }
  }

  public void OnSynchronized() {
    foreach (var composite in virtualVessels.Values) {
      composite.OnSynchronized();
    }
  }
}
