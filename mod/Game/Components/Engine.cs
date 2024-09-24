using System.Collections.Generic;
using System.Linq;
using Hgs.Core.Engine;
using Hgs.Core.Resources;
using Hgs.Core.Virtual;
using UnityEngine;

namespace Hgs.Game.Components;

public class Engine : VirtualComponent {
  public List<OperatingMode> Modes;
  public int selectedMode = 0;

  private OperatingMode activeMode => Modes[selectedMode];

  private BufferedRealtimeConsumer FuelConsumer;

  public float FixedUpdate_Thrust(float throttle, float atmPressure) {
    if (FuelConsumer == null || !FuelConsumer.TryConsumeDuringFixedUpdate(throttle, Time.fixedDeltaTime)) {
      return 0;
    }

    var activeIsp = activeMode.IspCurve.Evaluate((float) atmPressure);
    var activeMassFlow = activeMode.MaxMassFlowRate * throttle;
    var activeThrustN = activeIsp * activeMassFlow * OperatingMode.G;
    // Note: our thrust is in Newtons, but KSP uses kilonewtons for force, so divide by 1,000.
    var activeThrustKn = activeThrustN / 1000f;
    return activeThrustKn;
  }

  
  public override void OnActivate(VirtualVessel virtualVessel) {
    FuelConsumer = BufferedRealtimeConsumer.FromPropellantRecipe(virtualVessel, this, activeMode.Recipe, activeMode.MaxVolumetricFlow);
  }

  protected override void Load(ConfigNode node) {
    Modes = node.GetNodes("MODE").Select(OperatingMode.FromConfig).ToList();
  }

  protected override void Save(ConfigNode node) {
    foreach (var mode in Modes) {
      mode.SaveToConfig(node.AddNode("MODE"));
    }
  }
}