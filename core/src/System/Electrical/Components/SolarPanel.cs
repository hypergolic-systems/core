using Hgs.Core.Virtual;

namespace Hgs.Core.System.Electrical;

public class SolarPanel {

  // TODO: this is earth's sun (64E6)
  const double H_SUN = 64000000;

  float solarIrradiance = 0;

  public SolarPanel() {
  }
}
