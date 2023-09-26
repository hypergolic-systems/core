using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Hgs.Core.Simulation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hgs.Test.Simulation;

[TestClass]
public class ElectricFlowTest {

  private ResourceFlowSimulator sim;

  [TestInitialize]
  public void TestInitialize() {
    sim = new ResourceFlowSimulator(new ElectricFlowResolver());
  }

  [TestMethod]
  public void Test_OneInOneOut() {
    var prod = sim.Flow();
    prod.CanProduceRate = 10;
    var cons = sim.Flow();
    cons.CanConsumeRate = 5;

    sim.Reflow();

    AssertWithinEpsilon(-5, prod.ActiveRate);
    AssertWithinEpsilon(5, cons.ActiveRate);
  }

  [TestMethod]
  public void Test_OneSmallConsumer() {
    var prod1 = sim.Flow();
    var prod2 = sim.Flow();
    prod1.CanProduceRate = prod2.CanProduceRate = 10;
    
    var cons1 = sim.Flow();
    var cons2 = sim.Flow();
    var cons3 = sim.Flow();
    cons1.CanConsumeRate = cons3.CanConsumeRate = 10;
    cons2.CanConsumeRate = 6;

    sim.Reflow();

    // Producers should be tapped out.
    AssertWithinEpsilon(-10, prod1.ActiveRate);
    AssertWithinEpsilon(-10, prod2.ActiveRate);

    // Consumer 2 should be maxed out.
    AssertWithinEpsilon(6, cons2.ActiveRate);
    // Consumers 1 and 3 should have split the remaining 14:
    AssertWithinEpsilon(7, cons1.ActiveRate);
    AssertWithinEpsilon(7, cons3.ActiveRate);

    
  }

  private void AssertWithinEpsilon(double expected, double actual) {
    Assert.IsTrue(Math.Abs(expected - actual) < 0.0001, $"Expected {expected} but got {actual}.");
  }
}