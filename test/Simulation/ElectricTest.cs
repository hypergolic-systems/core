
using Hgs.Core.Simulator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hgs.Test.Simulation;

[TestClass]
public class ElectricTest {

[TestInitialize]
  public void TestInitialize() {
    SimulationDriver.Initialize();
    SimulationDriver.Instance.Sync(10);
  }

  [TestCleanup]
  public void TestCleanup() {
    SimulationDriver.Instance = null;
  }

  // [TestMethod, Timeout(5000)]
  // public void Test_Battery_with_Rtg() {
  //   var vessel = new FakeVessel();

  //   var batteryPart = vessel.AddPart(vessel.rootPart);
  //   batteryPart.AddModule(new FakeBatteryModule());

  //   var rtgPart = vessel.AddPart(vessel.rootPart);
  //   rtgPart.AddModule(new FakeRTGModule());

  //   var virtualVessel = new VirtualVessel();
  //   SimulationDriver.Instance.AddTarget(virtualVessel.resources[WellKnownResource.Electricity]);
  //   var battery = virtualVessel.virtualParts[batteryPart.persistentId].components.OfType<Battery>().First();

  //   // Advance 10 seconds
  //   SimulationDriver.Instance.Sync(15);

  //   Util.AssertWithinEpsilon(50, battery.Stored);

  //   SimulationDriver.Instance.Sync(115);

  //   Util.AssertWithinEpsilon(100, battery.Stored);
  // }
}
