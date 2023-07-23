using System;
using System.Collections.Generic;

namespace Hgs.Core.Virtual;

/**
 * The VesselShapeProcessor manages the relationship between a `CompositeSpacecraft` and
 * the actual game parts that make up the vessel.
 */
public abstract class VesselShapeProcessor {

  /**
    * Processes the vessel's game parts and subdivides them into a new
    * `CompositeSpacecraft` segment tree. The new composite will have a single
    * `Spacecraft` for the whole part tree.
    */
  public CompositeSpacecraft NewCompositeFromParts(object vessel) {
    var composite = new CompositeSpacecraft();

    var spacecraft = new Spacecraft();
    composite.spacecraft.Add(spacecraft);
    var rootSegment = new Segment();
    spacecraft.segments.Add(rootSegment);

    var rootPart = Adapter.Vessel_rootPart(vessel);
    this.IngestPartIntoSegmentTree(composite, spacecraft, rootSegment, rootPart, rootPart);
    return composite;
  }

  public bool CheckCompositeAgainstParts(CompositeSpacecraft composite, object vessel) {
    var dividers = new HashSet<SegmentDivider>(Adapter.Vessel_FindPartModulesImplementing<SegmentDivider>(vessel));

    foreach (var divider in dividers) {
      var otherSide = divider.OtherSide;

      // Ignore dividers with no second half.
      if (otherSide == null) {
        dividers.Remove(divider);
        continue;
      }

      var dividerId = Adapter.Part_persistentId(divider.Part);
      var otherSideId = Adapter.Part_persistentId(otherSide);
      var hasLink = composite.links.Find(link =>
        link.sideAPart == dividerId ||
        link.sideBPart == dividerId
      );

      // Check whether the link exists at all.
      if (hasLink == null) {
        // Fail: connected divider but no corresponding segment link.
        return false;
      }

      // Check whether the link correctly asserts the other side of the divider.
      if (hasLink.sideAPart != otherSideId && hasLink.sideBPart != otherSideId) {
        // Fail: link doesn't match divider.
        return false;
      }

      // The link matches the divider.
      dividers.Remove(divider);

      if (divider.DividerStyle == "dockingPort") {
        // The other side of the docking port could be a corresponding divider.
        var pairedDivider = Adapter.Part_FindModuleImplementing<SegmentDivider>(otherSide);
        // Only remove `pairedDivider` if it actually pairs back to this divider. It could be
        // that `pairedDivider` is a decoupler that decouples a third segment of the craft.
        if (pairedDivider != null && pairedDivider.OtherSide == divider.Part) {
          dividers.Remove(pairedDivider);
        }
      }
    }
    return true;
  }

  protected void IngestPartIntoSegmentTree(CompositeSpacecraft composite, Spacecraft craft, Segment segment, object part, object fromPart) {
    var divider = Adapter.Part_FindModuleImplementing<SegmentDivider>(part);
    var segmentForThisPart = segment;
    var segmentForChildParts = segment;
    if (divider != null) {
      var newSegment = new Segment();
      newSegment.craft = craft;
      craft.segments.Add(newSegment);
      
      var link = new SegmentLink();
      link.sideA = segment;
      link.sideB = newSegment;
      // This part is a divider, and needs special logic to branch the segment tree for recursion.
      if (divider.OtherSide == fromPart) {
        // We've just navigated across the divider, so it goes in a new segment.
        segmentForThisPart = newSegment;
        segmentForChildParts = newSegment;
        link.sideAPart = Adapter.Part_persistentId(fromPart);
        link.sideBPart = Adapter.Part_persistentId(part);
      } else {
        // We're about to cross the divider, so we're still in the current segment.
        segmentForChildParts = newSegment;
        link.sideAPart = Adapter.Part_persistentId(part);
        link.sideBPart = Adapter.Part_persistentId(divider.OtherSide);
      }
      segment.links.Add(link);
      newSegment.links.Add(link);
      composite.links.Add(link);
    }

    // Ingest the actual part (creating any virtual parts).
    this.IngestPartIntoSegment(composite, segmentForThisPart, part);

    // Process outgoing edges from this part to others. We actually don't know
    // how we arrived at `part` (either from parent -> child or from child -> parent).
    // So we need to consider both up and down the tree.
    var parent = Adapter.Part_parent(part);
    if (parent != null && parent != fromPart) {
      this.IngestPartIntoSegmentTree(composite, craft, segmentForChildParts, parent, part);
    }

    foreach (var childPart in Adapter.Part_children(part)) {
      if (childPart == fromPart) {
        continue;
      }
      this.IngestPartIntoSegmentTree(composite, craft, segmentForChildParts, childPart, part);
    }
  }

  protected abstract void IngestPartIntoSegment(CompositeSpacecraft composite, Segment segment, object part);
}
