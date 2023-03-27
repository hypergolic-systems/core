namespace Hgs.System.Electrical {

  public class SolarPanel : SimulatedPart, IProducer {
    // TODO: this is earth's sun (64E6)
    const double H_SUN = 64000000;

    float solarIrradiance = 0;


    public SolarPanel(uint partId) : base(partId) {}

    public Voltage GetVoltage() {
      return Voltage.Low;
    }

    public void OnCalculateProduction(uint seconds, Vessel vessel) {
      if (vessel.mainBody == Planetarium.fetch.Sun) {
        solarIrradiance = CalcuateSolarIrradiance(vessel);
      }
    }

    protected float CalcuateSolarIrradiance(Vessel vessel) {
      // (1 / r^2) * H_SUN
      // TODO: R_SUN is considered negligible here - do we care?
      // TODO: update this equation to calculate from Kerbin
      // TODO: what we really want is "fractional power of production at Kerbin"
      return (float) (H_SUN * (1 / vessel.orbit.pos.sqrMagnitude));
    }

    public int TryDrawPower(int wattDemand) {
      return 0;
    }

  }
}