using Hgs.Core.Virtual;

namespace Hgs.Core.System.Electrical;

public class SolarPanel : VirtualPart, PowerProducer {
  // TODO: this is earth's sun (64E6)
  const double H_SUN = 64000000;

  float solarIrradiance = 0;


  public SolarPanel(uint partId, uint index) : base(partId, index) {}

  public Voltage GetVoltage() {
    return Voltage.Low;
  }

  public ProducerKind Kind() {
    return ProducerKind.Free;
  }

  public void OnCalculateProduction(uint seconds, CompositeSpacecraft vessel) {
    // if (vessel.liveVessel.mainBody == Planetarium.fetch.Sun) {
    //   solarIrradiance = CalcuateSolarIrradiance(vessel.liveVessel);
    // }
  }

  // protected float CalcuateSolarIrradiance(Vessel liveVessel) {
  //   // (1 / r^2) * H_SUN
  //   // TODO: R_SUN is considered negligible here - do we care?
  //   // TODO: update this equation to calculate from Kerbin
  //   // TODO: what we really want is "fractional power of production at Kerbin"
  //   return (float) (H_SUN * (1 / liveVessel.orbit.pos.sqrMagnitude));
  // }

  public int TryDrawPower(int wattDemand) {
    return 0;
  }

}
