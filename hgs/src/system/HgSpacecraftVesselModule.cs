using System;
using Hgs.Part;
using Hgs.System.Electrical;
using UnityEngine;

namespace Hgs.System {

  /**
   * Provides the link between the simulated version of a `Spacecraft` with `SimulatedPart`s
   * and the game version of `Vessel`s.
   */
  public class HgSpacecraftVesselModule : VesselModule {

    /**
     * The `Spacecraft` associated with this vessel, or `null` if none exists.
     */
    public Spacecraft craft;

    protected override void OnAwake() {
      base.OnAwake();
    }

    public override void OnLoadVessel() {
      base.OnLoadVessel();

      if (vessel == null || vessel.parts == null) {
        return;
      }

      // The parts for this vessel just became available, so look for any parts that we need to simulate.
      foreach (var simModule in vessel.FindPartModulesImplementing<HgSimulatedPartModule>()) {
        var part = simModule.part;

        if (craft == null) {
          // Lazily instantiate the `Spacecraft` when we do have parts to simulate.
          craft = new Spacecraft();
        }

        if (!craft.parts.ContainsKey(part.persistentId)) {
          // This is a never-before-seen part, so instantiate it from the source `HgSimulatedPartModule`.
          var p = simModule.CreateSimulatedPart();
          craft.AddPart(p);
        }

        // Link the `Vessel` and `Spacecraft` worlds.
        var simPart = craft.parts[part.persistentId];
        simPart.simModule = simModule;
        simModule.simPart = simPart;

        simModule.OnLinkToSpacecraft(craft);
      }
    }

    public override void OnUnloadVessel() {
      if (vessel == null || vessel.parts == null || craft == null) {
        return;
      }

      // The `Vessel` associated with this `Spacecraft` is being unloaded, so disconnect the two.
      foreach (var simPart in craft.parts.Values) {
        var simModule = simPart.simModule;
        if (simModule == null) {
          continue;
        }

        simModule.OnUnlinkFromSpacecraft(craft);

        simModule.simPart = null;
        simPart.simModule = null;
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
      craft = new Spacecraft();
      foreach (var partNode in node.GetNodes("SIMULATED_PART")) {
        var simPart = LoadSimulatedPartFromConfig(partNode);
        if (simPart == null) {
          continue;
        }

        craft.AddPart(simPart);
      }
    }

    protected override void OnSave(ConfigNode node) {
      base.OnSave(node);

      if (craft == null || craft.parts.Count == 0) {
        // Nothing to save.
        return;
      }

      foreach (var part in craft.parts) {
        var partNode = node.AddNode("SIMULATED_PART");
        partNode.AddValue("id", part.Key.ToString());
        partNode.AddValue("type", part.Value.GetType().FullName);
        part.Value.Save(partNode);
      }
    }

    /**
     * Instantiate a `SimulatedPart` from its saved version in `node`.
     */
    protected SimulatedPart LoadSimulatedPartFromConfig(ConfigNode node) {
      SimulatedPart part = null;
      var id = uint.Parse(node.GetValue("id"));
      switch (node.GetValue("type")) {
        case "Hgs.System.Electrical.Battery":
          part = new Battery(id);
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
