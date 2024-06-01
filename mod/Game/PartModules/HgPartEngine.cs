using System.Collections.Generic;
using System.Linq;
using Hgs.Core.Virtual;
using UnityEngine;

public class HgPartEngine : HgVirtualPartModule {


  [KSPField]
  public float maxThrust = 0f;

  [KSPField]
  private string thrustVectorTransformName = "thrustTransform";

  private List<Transform> thrustTransforms;

  public override void InitializeComponents() {
  }

  public override void OnStart(StartState state) {
    base.OnStart(state);
    thrustTransforms = this.thrustTransforms = this.part.FindModelTransforms(this.thrustVectorTransformName).ToList();
  }

  bool active = false;

  [KSPEvent(guiActive = true, guiActiveEditor = false, guiName = "Test!")]
  public void Activate() {
    active = true;
  }

  public void FixedUpdate() {
    if (active) {
      float finalThrust = maxThrust * this.vessel.ctrlState.mainThrottle;
      var multiplier = 1f / thrustTransforms.Count;
      foreach (var thrustTransform in thrustTransforms) {
        var thrust = -thrustTransform.forward * finalThrust * multiplier;
        this.part.AddForceAtPosition(thrust, thrustTransform.position);
      }
    }
  }
}
