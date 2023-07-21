using System;
using System.Collections.Generic;
using Hgs.Core.Virtual;
using Hgs.Core.System.Electrical;
using Hgs.Mod.Part;

namespace Hgs.Virtual {

  /**
   * Provides the link between the simulated version of a `Spacecraft` with `SimulatedPart`s
   * and the game version of `Vessel`s.
   */
  public class HgVirtualVesselModule : VesselModule {

    /**
     * The `Spacecraft` associated with this vessel, or `null` if none exists.
     */
    public CompositeSpacecraft craft;

    protected override void OnAwake() {
      base.OnAwake();
    }

    public override void OnLoadVessel() {
      base.OnLoadVessel();

      if (vessel == null || vessel.parts == null) {
        return;
      }

      // The parts for this vessel just became available, so look for any parts that we need to simulate.
      foreach (var virtualModule in vessel.FindPartModulesImplementing<VirtualizedModule>()) {
        var part = (virtualModule.module as PartModule).part;

        if (craft == null) {
          // Lazily instantiate the `Spacecraft` when we do have parts to simulate.
          craft = CompositeSpacecraft.fromLiveVessel(vessel);
        }

        // if (!craft.virtualPartsMap.ContainsKey(part.persistentId)) {
        //   // This is a never-before-seen part, so instantiate it from the source `HgSimulatedPartModule`.
        //   virtualModule.InitializeVirtualParts();
        //   craft.virtualPartsMap[part.persistentId] = virtualModule.virtualParts;
        // } else {
        //   virtualModule.virtualParts = craft.virtualPartsMap[part.persistentId];
        // }

        // Link the `Vessel` and `Spacecraft` worlds.
        foreach (var virtualPart in virtualModule.virtualParts) {
          virtualPart.liveModule = virtualModule;
        }

        virtualModule.OnLinkToSpacecraft(craft);
      }
    }

    public override void OnUnloadVessel() {
      if (vessel == null || vessel.parts == null || craft == null) {
        return;
      }

      foreach (var module in vessel.FindPartModulesImplementing<VirtualizedModule>()) {
        module.OnUnlinkFromSpacecraft(craft);
        foreach (var virtualPart in module.virtualParts) {
          virtualPart.liveModule = null;
        }
      }

      // Run at the end, just in case.
      base.OnUnloadVessel();
    }

    protected override void OnLoad(ConfigNode node) {
      base.OnLoad(node);
      if (!node.HasNode("SIMULATED_PART")) {
        return;
      }

      // There are `SimulatedPart`s saved for this spacecraft, so instantiate them from the save file.
      craft = CompositeSpacecraft.fromLiveVessel(vessel);
      foreach (var moduleNode in node.GetNodes("VIRTUALIZED_MODULE")) {
        var partId = uint.Parse(node.GetValue("id"));
        var partNodes = moduleNode.GetNodes("PART");
        var virtualParts = new List<VirtualPart>(partNodes.Length);
        foreach (var partNode in partNodes) {
          var index = uint.Parse(partNode.GetValue("index"));
          var virtualPart = LoadSimulatedPartFromConfig(partId, index, partNode);
          if (virtualPart == null) {
            continue;
          }
          virtualParts.Add(virtualPart);
        }

        // craft.virtualPartsMap[partId] = virtualParts;
      }
    }

    protected override void OnSave(ConfigNode node) {
      base.OnSave(node);

      // if (craft == null || craft.virtualPartsMap.Count == 0) {
      //   // Nothing to save.
      //   return;
      // }

      // foreach (var part in craft.virtualPartsMap) {
      //   var moduleNode = node.AddNode("VIRTUALIZED_MODULE");
      //   moduleNode.AddValue("id", part.Key.ToString());
      //   foreach (var virtualPart in part.Value) {
      //     var partNode = moduleNode.AddNode("PART");
      //     partNode.AddValue("index", virtualPart.index.ToString());
      //     partNode.AddValue("type", virtualPart.GetType().FullName);
      //     virtualPart.Save(partNode);
      //   }
      // }
    }

    /**
     * Instantiate a `SimulatedPart` from its saved version in `node`.
     */
    protected VirtualPart LoadSimulatedPartFromConfig(uint partId, uint index, ConfigNode node) {
      VirtualPart part = null;
      switch (node.GetValue("type")) {
        case "Hgs.System.Electrical.Battery":
          part = new Battery(partId, index);
          break;
        default:
          throw new Exception(string.Format("Unknown SimulatedPart: {0}", node.GetValue("type")));
      }
      if (part != null) {
        part.Load(node);
      }
      return part;
    }
  }
}
