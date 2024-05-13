using System;
using System.Diagnostics;
using Hgs.Core.Simulation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hgs.Test.Simulation;

[TestClass]
public class SimulationDriverTest {


  [TestInitialize]
  public void TestInitialize() {
    SimulationDriver.Instance = new SimulationDriver();
    // Start at t = 10
    // TODO: mock time properly
    SimulationDriver.Instance.Sync(10);
  }

  [TestMethod]
  public void Test_SingleTarget() {
    var target = new MockTarget();
    SimulationDriver.Instance.AddTarget(target);

    // Ticking the test by 5s should produce a value of 5.
    SimulationDriver.Instance.Sync(15);
    AssertWithinEpsilon(5, target.Value);

    // Ticking the test by 10s should cause a recalculation after 5s, and `Value` should be
    // (10, for the first 10s) + (10, for the second 5s) = 20.
    SimulationDriver.Instance.Sync(25);
    AssertWithinEpsilon(20, target.Value);
  }


  private void AssertWithinEpsilon(double expected, double actual) {
    Assert.IsTrue(Math.Abs(expected - actual) < 0.0001, $"Expected {expected} but got {actual}.");
  }

  class MockTarget : ISimulated {
    public double Value = 0;
    double Rate = 1;

    public bool IsDirty { get; set; } = false;

    public void RecomputeState() {
      Debug.Assert(IsDirty);
      IsDirty = false;

      // The Rate doubles every 10 seconds.
      Rate *= 2;
      RemainingValidDeltaT = 10;
    }

    public double RemainingValidDeltaT { get; set; } = 10;

    public void Tick(double deltaT) {
      RemainingValidDeltaT -= deltaT;
      if (Math.Abs(RemainingValidDeltaT) < 0.0001) {
        RemainingValidDeltaT = 0;
        IsDirty = true;
      }

      Value += Rate * deltaT;
    }

    public void OnSynchronized() {
    }
  }
}