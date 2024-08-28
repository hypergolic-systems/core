using Hgs.Core.Simulator;
using Hgs.Core.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hgs.Core.Engine;
using Hgs.Test.Util;
using UnityEngine;
using System;

namespace Hgs.Test.Resources;

[TestClass]
public class BufferedResourceConsumerTest {

  private TestEnvironment testEnv;
  private TestVessel vessel;

  [TestInitialize]
  public void Initialize() {
    testEnv = new TestEnvironment();

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
    var behaviour = new TestBehaviour(consumer);
    testEnv.Behaviours.Add(behaviour);

    // The request should pass because we're not requesting any resources.
    testEnv.TickSeconds(1);
    Assert.IsTrue(behaviour.ResourcesAvailable);

    // This configuration should be infinitely stable.
    Assert.AreEqual(ulong.MaxValue, vessel.resources[Resource.LiquidFuel].RemainingValidDeltaT);
  }

  [TestMethod]
  public void TestRequest_NoResources() {
    var consumer = BufferedRealtimeConsumer.FromPropellantRecipe(vessel, vessel.AddTestEngine(), PropellantRecipe.LF_LOX, 16);
    var behaviour = new TestBehaviour(consumer);
    testEnv.Behaviours.Add(behaviour);

    // The request should fail, because the buffer is empty.
    behaviour.LiveRateFraction = 1;
    testEnv.TickSeconds(1);

    Assert.IsFalse(behaviour.ResourcesAvailable);

    // This configuration should be infinitely stable, as if nothing changes then the buffer will
    // never fill.
    Assert.AreEqual(ulong.MaxValue, vessel.resources[Resource.LiquidFuel].RemainingValidDeltaT);
  }

  [TestMethod]
  public void TestRequest_InitialFill() {
    var lfo = vessel.AddTestTank(Resource.LiquidFuel, 100);
    var lox = vessel.AddTestTank(Resource.LiquidOxygen, 100);
    var consumer = BufferedRealtimeConsumer.FromPropellantRecipe(vessel, vessel.AddTestEngine(), PropellantRecipe.LF_LOX, 16);
    var behaviour = new TestBehaviour(consumer);
    testEnv.Behaviours.Add(behaviour);

    // The request should pass because we're not requesting any resources, but it should start
    // buffering.
    testEnv.TickSeconds(1);
    Assert.IsTrue(behaviour.ResourcesAvailable);

    // This configuration should be infinitely stable as the buffer is full.
    AssertUtil.WithinEpsilon(vessel.resources[Resource.LiquidFuel].RemainingValidDeltaT, ulong.MaxValue);
    
    // We should have grabbed 12L of LiquidFuel and 4L of LiquidOxygen.
    AssertUtil.WithinEpsilon(100 - 12, lfo.Amount);
    AssertUtil.WithinEpsilon(consumer.GetAmountInBuffer(Resource.LiquidFuel), 12);
    AssertUtil.WithinEpsilon(100 - 4, lox.Amount);
    AssertUtil.WithinEpsilon(consumer.GetAmountInBuffer(Resource.LiquidOxygen), 4);
  }

  [TestMethod]
  public void TestRequest_InitialFillThenSteadyState() {
    var lfo = vessel.AddTestTank(Resource.LiquidFuel, 100);
    var lox = vessel.AddTestTank(Resource.LiquidOxygen, 100);
    var consumer = BufferedRealtimeConsumer.FromPropellantRecipe(vessel, vessel.AddTestEngine(), PropellantRecipe.LF_LOX, 16);
    var behaviour = new TestBehaviour(consumer);
    testEnv.Behaviours.Add(behaviour);

    // Initial fill.
    testEnv.TickSeconds(1);

    // Burn at half power for 1 second.
    behaviour.LiveRateFraction = 0.5f;
    testEnv.TickSeconds(1);

    // Tanks should still be full.
    AssertUtil.WithinEpsilon(12, consumer.GetAmountInBuffer(Resource.LiquidFuel));
    AssertUtil.WithinEpsilon(4, consumer.GetAmountInBuffer(Resource.LiquidOxygen));

    // We should have consumed 18L of LiquidFuel and 6L of LiquidOxygen:
    // - Initial fill: 12L / 4L
    // - 50% throttle for 1s: 6L / 2L
    AssertUtil.WithinEpsilon(100 - 18, lfo.Amount);
    AssertUtil.WithinEpsilon(100 - 6, lox.Amount);
  }

  class TestBehaviour : MonoBehaviour {
    BufferedRealtimeConsumer Consumer;
    public float LiveRateFraction = 0;
    float LastUpdate = 0;

    public bool ResourcesAvailable = true;
    public TestBehaviour(BufferedRealtimeConsumer consumer) {  
      Consumer = consumer;
      LastUpdate = (float) Planetarium.GetUniversalTime();
    }

    void FixedUpdate() {
      var timeNow = (float) Planetarium.GetUniversalTime();
      float delta = timeNow - LastUpdate;
      ResourcesAvailable = Consumer.TryConsumeDuringFixedUpdate(LiveRateFraction, delta);
      LastUpdate = timeNow;
    }
  }
}
