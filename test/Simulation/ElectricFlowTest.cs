using Hgs.Core.Resources;
using Hgs.Core.Resources.Resolvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hgs.Test.Simulation;

[TestClass]
public class ElectricFlowTest {

  private ResourceSystem sim;

  [TestInitialize]
  public void TestInitialize() {
    sim = new ResourceSystem(new ElectricFlowResolver());
  }

  [TestMethod]
  public void Test_OneInOneOut() {
    var prod = sim.NewFlow();
    prod.CanProduceRate = 10;
    var cons = sim.NewFlow();
    cons.CanConsumeRate = 5;

    sim.RecomputeState();

    Util.AssertWithinEpsilon(-5, prod.ActiveRate);
    Util.AssertWithinEpsilon(5, cons.ActiveRate);
  }

  [TestMethod]
  public void Test_OneSmallConsumer() {
    var prod1 = sim.NewFlow();
    var prod2 = sim.NewFlow();
    prod1.CanProduceRate = prod2.CanProduceRate = 10;
    
    var cons1 = sim.NewFlow();
    var cons2 = sim.NewFlow();
    var cons3 = sim.NewFlow();
    cons1.CanConsumeRate = cons3.CanConsumeRate = 10;
    cons2.CanConsumeRate = 6;

    sim.RecomputeState();

    // Producers should be tapped out.
    Util.AssertWithinEpsilon(-10, prod1.ActiveRate);
    Util.AssertWithinEpsilon(-10, prod2.ActiveRate);

    // Consumer 2 should be maxed out.
    Util.AssertWithinEpsilon(6, cons2.ActiveRate);
    // Consumers 1 and 3 should have split the remaining 14:
    Util.AssertWithinEpsilon(7, cons1.ActiveRate);
    Util.AssertWithinEpsilon(7, cons3.ActiveRate);

    
  }
}