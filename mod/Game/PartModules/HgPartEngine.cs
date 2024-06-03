using System;
using System.Collections.Generic;
using System.Linq;
using Hgs.Core.Engine;
using Hgs.Core.Virtual;
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
  }

  public override void OnStart(StartState state) {
    base.OnStart(state);
    thrustTransforms = this.thrustTransforms = this.part.FindModelTransforms(this.thrustVectorTransformName).ToList();
    Debug.Assert(modes.Count > 0, "No recipes found for engine");
  }

  bool active = false;

  [KSPEvent(guiActive = true, guiActiveEditor = false, guiName = "Test Engine")]
  public void Activate() {
    active = true;
  }

  public void FixedUpdate() {
    if (active) {
      var activeIsp = activeMode.IspCurve.Evaluate((float) part.staticPressureAtm);
      var activeMassFlow = activeMode.MaxMassFlowRate * this.vessel.ctrlState.mainThrottle;
      var activeThrustN = activeIsp * activeMassFlow * OperatingMode.G;
      // Note: our thrust is in Newtons, but KSP uses kilonewtons for force, so divide by 1,000.
      var activeThrustKn = activeThrustN / 1000f;
      var multiplier = 1f / thrustTransforms.Count;

      var label = $"Mass: {Math.Round(activeMassFlow, 2)} kg/s\nIsp: {Math.Round(activeIsp, 2)}";
      foreach (var ing in activeMode.Recipe.Ingredients) {
        label += $"\n{ing.Resource.Name}: {Math.Round(ing.VolumetricFlowPerUnitMass * activeMassFlow, 2)} L/s";
      }
      data = label;

      foreach (var thrustTransform in thrustTransforms) {
        var thrust = -thrustTransform.forward * activeThrustKn * multiplier;
        this.part.AddForceAtPosition(thrust, thrustTransform.position);
      }
    }
  }

  public override void LoadStaticData(ConfigNode config) {
    base.LoadStaticData(config);
    modes = config.GetNodes("MODE").Select(OperatingMode.FromConfig).ToList();
  }
}
