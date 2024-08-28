using Hgs.Core.Resources;
using Hgs.Test.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hgs.Test.Simulation;

[TestClass]
public class ElectricFlowTest {

  private ResourceSystem sim;

  [TestInitialize]
  public void TestInitialize() {
  sim = new ResourceSystem();
  }

  [TestMethod]
  public void Test_OneInOneOut() {
    var prod = new TestProducer {
      DynamicProductionLimit = 10,
    };
    sim.AddProducer(prod);

    var cons = sim.NewTicket();
    cons.Request = 5;

    sim.RecomputeState();

    AssertUtil.WithinEpsilon(5, prod.DynamicProductionRate);
    AssertUtil.WithinEpsilon(5, cons.Rate);
  }

  [TestMethod]
  public void Test_OneSmallConsumer() {
    var prod1 = new TestProducer {
      DynamicProductionLimit = 10,
    };
    var prod2 = new TestProducer {
      DynamicProductionLimit = 10,
    };
    sim.AddProducer(prod1);
    sim.AddProducer(prod2);

    var cons1 = sim.NewTicket();
    cons1.Request = 10;
    var cons2 = sim.NewTicket();
    cons2.Request = 6;
    var cons3 = sim.NewTicket();
    cons3.Request = 10;


    sim.RecomputeState();

    // Producers should be maxed out.
    AssertUtil.WithinEpsilon(10, prod1.DynamicProductionRate);
    AssertUtil.WithinEpsilon(10, prod2.DynamicProductionRate);

    // Consumers 1 and 2 should be respectively maxed out.
    AssertUtil.WithinEpsilon(10, cons1.Rate);
    AssertUtil.WithinEpsilon(6, cons2.Rate);

    // Consumer 3 should have gotten the remaining allocation.
    AssertUtil.WithinEpsilon(4, cons3.Rate);
  }

  public class TestProducer : ResourceSystem.IProducer {
    public float BaselineProduction { get; set; } = 0;
    public float DynamicProductionRate { get; set; } = 0;
    public float DynamicProductionLimit { get; set; } =  0;
    public ulong RemainingValidDeltaT { get; set; } = ulong.MaxValue;
    public int Priority { get; set; } = 0;
    public void Commit() {}
    public void Tick(ulong deltaT) {}
  }
}