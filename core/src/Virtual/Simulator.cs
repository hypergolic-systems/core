using Hgs.Core.System.Electrical;

namespace Hgs.Core.Virtual;

public class Simulator {

  protected Bus lowVoltageBus;
  protected Bus highVoltageBus;

  protected CompositeSpacecraft composite;

  public Simulator(CompositeSpacecraft composite) {
    this.composite = composite;
    this.lowVoltageBus = Bus.FromSpacecraft(composite, Voltage.Low);
    this.highVoltageBus = Bus.FromSpacecraft(composite, Voltage.High);
  }
  public void Simulate(uint seconds) {
    lowVoltageBus.PreTick(seconds, composite);
    highVoltageBus.PreTick(seconds, composite);
    lowVoltageBus.Tick(seconds, composite);
    highVoltageBus.Tick(seconds, composite);
    lowVoltageBus.PostTick(seconds, composite);
    highVoltageBus.PostTick(seconds, composite);
  }
}