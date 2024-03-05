
using System;
using System.CodeDom;
using System.Linq;
using Hgs.Core;
using Hgs.Core.Simulation;
using Hgs.Core.System.Electrical.Components;
using Hgs.Core.Virtual;
using Hgs.Test.FakeKSP;
using Hgs.Test.FakeMod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hgs.Test.Simulation;

[TestClass]
public class ElectricTest {

[TestInitialize]
  public void TestInitialize() {
    Adapter.Instance = new FakeKSPAdapter();
    SimulationDriver.Initialize();
  }

  [TestCleanup]
  public void TestCleanup() {
    SimulationDriver.Instance.Shutdown();
    SimulationDriver.Instance = null;
  }

  [TestMethod, Timeout(5000)]
  public void Test_Battery_with_Rtg() {
    var manager = new CompositeManager();
    var vessel = new FakeVessel();

    var batteryPart = vessel.AddPart(vessel.rootPart);
    batteryPart.AddModule(new FakeBatteryModule());

    var rtgPart = vessel.AddPart(vessel.rootPart);
    rtgPart.AddModule(new FakeRTGModule());

    var composite = manager.OnLoadVessel(vessel);
    var battery = composite.partMap[batteryPart.persistentId].components.OfType<Battery>().First();

    // Advance 10 seconds
    SimulationDriver.Instance.RaiseUpperBoundOfTime(5);
    SimulationDriver.Instance.Sync();

    Util.AssertWithinEpsilon(50, battery.Stored);

    SimulationDriver.Instance.RaiseUpperBoundOfTime(105);
    SimulationDriver.Instance.Sync();

    Util.AssertWithinEpsilon(100, battery.Stored);
  }
}
