using System;
using Hgs.Core.Virtual;

namespace Hgs.Game.Components.Electrical;

public class SolarPanel : VirtualComponent {

  // TODO: this is earth's sun (64E6)
  const double H_SUN = 64000000;

  float solarIrradiance = 0;

  public SolarPanel() {
  }

  protected override void Load(ConfigNode node) {
    throw new NotImplementedException();
  }

  protected override void Save(ConfigNode node) {
    throw new NotImplementedException();
  }

  public override void OnActivate(VirtualVessel virtualVessel) {
    throw new NotImplementedException();
  }
}
