using System;
using System.Collections.Generic;
using System.Linq;
using Hgs.Core.Engine;
using Hgs.Core.Virtual;
using Hgs.Game.Components;
using UnityEngine;

public class HgPartEngine : HgVirtualPartModule {
  [KSPField]
  private string thrustVectorTransformName = "thrustTransform";

  [KSPField(isPersistant = false, guiActive = true)]
  [UI_Label]
  public string data = "";


  private List<Transform> thrustTransforms;

  [StaticField]
  public List<OperatingMode> modes;
  
  private int activeModeIndex = 0;

  private OperatingMode activeMode => modes[activeModeIndex];

  public override void InitializeComponents() {
    VirtualPart.AddComponent(new Engine {
      Modes = modes,
    });
  }

  public override void OnStart(StartState state) {
    base.OnStart(state);
    thrustTransforms = this.thrustTransforms = this.part.FindModelTransforms(this.thrustVectorTransformName).ToList();
    Debug.Assert(modes?.Count > 0, "No recipes found for engine");
  }

  bool active = false;

  [KSPEvent(guiActive = true, guiActiveEditor = false, guiName = "Test Engine")]
  public void Activate() {
    active = true;
  }

  public void FixedUpdate() {
    var engine = this.VirtualPart.Components[0] as Engine;
    if (active) {
      var totalThrust = engine.FixedUpdate_Thrust(this.vessel.ctrlState.mainThrottle, (float) part.staticPressureAtm);
      var multiplier = 1f / thrustTransforms.Count;

      data = $"Thrust: {totalThrust} kN";
      // var label = $"Mass: {Math.Round(activeMassFlow, 2)} kg/s\nIsp: {Math.Round(activeIsp, 2)}\nThrust: {Math.Round(activeThrustKn, 2)} kN";
      // foreach (var ing in activeMode.Recipe.Ingredients) {
      //   label += $"\n{ing.Resource.Name}: {Math.Round(ing.VolumetricFlowPerUnitMass * activeMassFlow, 2)} L/s";
      // }
      // data = label;

      foreach (var thrustTransform in thrustTransforms) {
        var nozzleThrust = -thrustTransform.forward * totalThrust * multiplier;
        this.part.AddForceAtPosition(nozzleThrust, thrustTransform.position);
      }
    }
  }

  public override void LoadStaticData(ConfigNode config) {
    Util.Log("[HgPartEngine.LoadStaticData] " + config.ToString());
    base.LoadStaticData(config);
    modes = config.GetNodes("MODE").Select(OperatingMode.FromConfig).ToList();
  }
}
