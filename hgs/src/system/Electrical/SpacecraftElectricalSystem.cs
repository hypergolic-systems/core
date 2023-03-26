namespace Hgs.System.Electrical {

  public class SpacecraftElectricalSystem {
    public Bus hvSystem = new Bus(Voltage.High);
    public Bus lv = new Bus(Voltage.Low);


  }

  class LvFromHvLink : IProducer {
    IBus hvBus;

    public Voltage GetVoltage() {
      return Voltage.Low;
    }

    public LvFromHvLink(IBus hvBus) {
      this.hvBus = hvBus;
    }

    public void OnCalculateProduction(uint seconds) {
      // no calculations necessary.
    }

    public int TryDrawPower(int wattsNeeded) {
      return hvBus.TryDrawPower(wattsNeeded);
    }
  }
}