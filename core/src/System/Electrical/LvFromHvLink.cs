using Hgs.Core.Virtual;

namespace Hgs.Core.System.Electrical;

public class LvFromHvLink : PowerProducer {
  Bus hvBus;

  public ProducerKind Kind() {
    return ProducerKind.HighVoltageBridge;
  }

  public Voltage GetVoltage() {
    return Voltage.Low;
  }

  public LvFromHvLink(Bus hvBus) {
    this.hvBus = hvBus;
  }

  public void OnCalculateProduction(uint seconds, CompositeSpacecraft vessel) {
    // no calculations necessary.
  }

  public int TryDrawPower(int wattsNeeded) {
    return hvBus.TryDrawPower(wattsNeeded);
  }
}
