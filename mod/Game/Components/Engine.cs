using System.Collections.Generic;
using System.Linq;
using Hgs.Core.Engine;
using Hgs.Core.Virtual;

namespace Hgs.Game.Components;

public class Engine : VirtualComponent {

  public List<OperatingMode> Modes;
  public int selectedMode = 0;

  private OperatingMode activeMode => Modes[selectedMode];

  public float FixedUpdate_Thrust(float throttle, float atmPressure) {
    var activeIsp = activeMode.IspCurve.Evaluate((float) atmPressure);
    var activeMassFlow = activeMode.MaxMassFlowRate * throttle;
    var activeThrustN = activeIsp * activeMassFlow * OperatingMode.G;
    // Note: our thrust is in Newtons, but KSP uses kilonewtons for force, so divide by 1,000.
    var activeThrustKn = activeThrustN / 1000f;
    return activeThrustKn;
  }

  
  public override void OnActivate(VirtualVessel virtualVessel) {
    throw new System.NotImplementedException();
  }

  protected override void Load(ConfigNode node) {
    Modes = node.GetNodes("MODE").Select(OperatingMode.FromConfig).ToList();
  }

  protected override void Save(ConfigNode node) {
    var modes = node.AddNode("MODES");
    foreach (var mode in Modes) {
      mode.SaveToConfig(modes.AddNode("MODE"));
    }
  }
}