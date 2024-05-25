using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hgs.Core.Simulation;

namespace Hgs.Core.Virtual;

/// <summary>
/// Manages `Composite` instances, which are the virtual representations of physical
/// vessels.
/// </summary>
public class CompositeManager {

  protected Dictionary<uint, Composite> composites = new();
  protected Dictionary<string, Type> componentTypes = new();


  public static CompositeManager Instance = new CompositeManager();

  public void RegisterComponentType(Type type) {
    componentTypes[type.FullName] = type;
  }

  public Composite GetSpacecraft(object vessel) {
    var vesselId = Adapter.Vessel_persistentId(vessel);
    return composites.ContainsKey(vesselId) ? composites[vesselId] : null;
  }

  public Composite OnLoadVessel(object vessel) {
    var id = Adapter.Vessel_persistentId(vessel);
    Adapter.Log($"Loading vessel {id}");
    // `vessel` was just loaded physically, and now has parts.
    if (!composites.ContainsKey(id)) {
      // This vessel has never before been seen, so create it freshly from the parts themselves.
      composites[id] = new Composite(id);
    }

    var composite = composites[id];
    composite.liveVessel = vessel;
    composite.Clear();
    this.LoadStructureFromVessel(composite, vessel);

    // Keep track of which parts were here from the start, so we know which ones didn't match any
    // `SimulatedModule`s and should be removed.
    var existingVirtualParts = new HashSet<uint>(composite.partMap.Keys);

    // Keep track of which parts were newly created. That's because we need to initialize those
    // parts when we find `SimulatedModule`s for them.
    var newParts = new HashSet<uint>();

    foreach (var module in Adapter.Vessel_FindPartModulesImplementing<VirtualModule>(vessel)) {
      var partId = Adapter.Part_persistentId(module.gamePart);

      if (!composite.partMap.ContainsKey(partId)) {
        composite.partMap[partId] = new VirtualPart {
          id = partId,
        };
      }

      var part = composite.partMap[partId];
      module.virtualPart = part;
      if (!part.components.Any(component => module.OwnsComponent(component))) {
        // This module owns no components on this part, so see if it wants to create some.
        module.InitializeComponents(composite, part);
      }

      // Associate the module with the components it owns.
      foreach (var component in part.components) {
        if (module.OwnsComponent(component)) {
          component.virtualModule = module;
        }
        component.OnAttached(composite);
      }
 
      module.OnLinkToSpacecraft(composite);
      existingVirtualParts.Remove(partId);
    }

    foreach (var oldPartId in existingVirtualParts) {
      // These parts didn't have corresponding physical parts. Remove them from the spacecraft.
      composite.partMap.Remove(oldPartId);
    }

    if (SimulationDriver.Instance != null) {
      foreach (var resource in composite.resources.Values) {
        SimulationDriver.Instance.AddTarget(resource);
      }
    }

    return composite;
  }

  public void OnUnloadVessel(object vessel) {
    var id = Adapter.Vessel_persistentId(vessel);
    if (!composites.ContainsKey(id)) {
      return;
    }

    var composite = composites[id];
    foreach (var module in Adapter.Vessel_FindPartModulesImplementing<VirtualModule>(vessel)) {
      module.OnUnlinkFromSpacecraft(composite);
      foreach (var component in module.virtualPart.components) {
        component.virtualModule = null;
      }
    }
    composite.liveVessel = null;
  }

  public void OnLoadVesselConfig(object node) {
    var compositeNode = Adapter.ConfigNode_Get(node, "HGS_COMPOSITE");
    if (compositeNode == null) {
      // There is no serialized Composite associated with this vessel (yet).
      return;
    }

    var id = uint.Parse(Adapter.ConfigNode_Get(compositeNode, "id"));
    var composite = new Composite(id);
    composites[id] = composite;

    this.LoadStructureFromConfig(composite, compositeNode);
  }

  protected void LoadStructureFromConfig(Composite composite, object node) {
    foreach (var craftNode in Adapter.ConfigNode_GetNodes(node, "SPACECRAFT")) {
      var craft = new Spacecraft {
        composite = composite,
      };
      composite.spacecraft.Add(craft);

      foreach (var segmentNode in Adapter.ConfigNode_GetNodes(craftNode, "SEGMENT")) {
        var definingPart = uint.Parse(Adapter.ConfigNode_Get(segmentNode, "definingPart"));
        var segment = new Segment(craft, definingPart);
        craft.segmentsByDefiningPart[definingPart] = segment;

        foreach (var partNode in Adapter.ConfigNode_GetNodes(segment, "PART")) {
          var part = new VirtualPart {
            id = uint.Parse(Adapter.ConfigNode_Get(partNode, "id")),
          };
          foreach (var componentNode in Adapter.ConfigNode_GetNodes(partNode, "COMPONENT")) {
            var typeString = Adapter.ConfigNode_Get(partNode, "type");
            if (!componentTypes.ContainsKey(typeString)) {
              throw new Exception(string.Format("Unknown VirtualComponent type: {0}", typeString));
            }

            var component = (VirtualComponent) Activator.CreateInstance(componentTypes[typeString]);
            component.partId = part.id;
            component.index = int.Parse(Adapter.ConfigNode_Get(partNode, "index"));

            segment.parts[part.id] = part;
            composite.partMap[part.id] =  part;
          }
        }
      }
    }
  }

  /// <summary>
  /// Extracts the structure of a `Composite` from the live parts.
  /// </summary>
  protected void LoadStructureFromVessel(Composite composite, object vessel) {
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
  protected void IngestPartIntoSegmentTree(Composite composite, Spacecraft craft, Segment segment, object part, object fromPart) {
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
    foreach (var composite in composites.Values) {
      composite.OnSynchronized();
    }
  }
}
