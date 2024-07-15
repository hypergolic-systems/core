using Hgs.Core.Simulator;
using Hgs.Core.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hgs.Core.Virtual;
using Hgs.Core.Engine;
using System.Diagnostics;
using Hgs.Game.Components.Tankage;
using Hgs.Test.Util;

namespace Hgs.Test.Resources;

[TestClass]
public class BufferedResourceConsumerTest {

  private TestVessel vessel;

  [TestInitialize]
  public void Initialize() {
    SimulationDriver.Initialize();
    SimulationDriver.Instance.Sync(1);

    vessel = new TestVessel();
    foreach (var system in vessel.resources.Values) {
      SimulationDriver.Instance.AddTarget(system);
    }
  }

  [TestMethod]
  public void TestCreation() {
    var consumer = BufferedRealtimeConsumer.FromPropellantRecipe(vessel, vessel.AddTestEngine(), PropellantRecipe.LF_LOX, 16);
    Assert.IsNotNull(consumer);
  }

  [TestMethod]
  public void TestRequest_ZeroFraction() {
    var consumer = BufferedRealtimeConsumer.FromPropellantRecipe(vessel, vessel.AddTestEngine(), PropellantRecipe.LF_LOX, 16);
    // The request should pass because we're not requesting any resources.
    Assert.IsTrue(consumer.TryConsumeDuringFixedUpdate(0, 1));
    SimulationDriver.Instance.Sync(2);

    // This configuration should be infinitely stable.
    Assert.AreEqual(double.MaxValue, vessel.resources[Resource.LiquidFuel].RemainingValidDeltaT);
  }

  [TestMethod]
  public void TestRequest_NoResources() {
    var consumer = BufferedRealtimeConsumer.FromPropellantRecipe(vessel, vessel.AddTestEngine(), PropellantRecipe.LF_LOX, 16);
    // The request should fail, because the buffer is empty.
    Assert.IsFalse(consumer.TryConsumeDuringFixedUpdate(1, 1));
    SimulationDriver.Instance.Sync(2);

    // This configuration should be infinitely stable, as if nothing changes then the buffer will
    // never fill.
    Assert.AreEqual(double.MaxValue, vessel.resources[Resource.LiquidFuel].RemainingValidDeltaT);
  }

  [TestMethod]
  public void TestRequest_InitialFill() {
    var lfo = vessel.AddTestTank(Resource.LiquidFuel, 100);
    var lox = vessel.AddTestTank(Resource.LiquidOxygen, 100);
    var consumer = BufferedRealtimeConsumer.FromPropellantRecipe(vessel, vessel.AddTestEngine(), PropellantRecipe.LF_LOX, 16);

    // The request should pass because we're not requesting any resources, but it should start
    // buffering.
    Assert.IsTrue(consumer.TryConsumeDuringFixedUpdate(0, 1));
    SimulationDriver.Instance.Sync(2);

    // This configuration should be infinitely stable as the buffer is full.
    Assert.AreEqual(vessel.resources[Resource.LiquidFuel].RemainingValidDeltaT, double.MaxValue);
    
    // We should have grabbed 12L of LiquidFuel and 4L of LiquidOxygen.
    Assert.AreEqual(100 - 12, lfo.Amount);
    Assert.AreEqual(100 - 4, lox.Amount);
  }
}
