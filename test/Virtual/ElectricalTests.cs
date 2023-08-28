using System.Diagnostics;
using System.Runtime.InteropServices;
using Hgs.Core;
using Hgs.Core.System.Electrical.Components;
using Hgs.Core.Virtual;
using Hgs.Test.FakeKSP;
using Hgs.Test.FakeMod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hgs.Test.Virtual;

[TestClass]
public class ElectricalTests {

  private SpacecraftManager manager;

  [TestInitialize]
  public void TestInitialize() {
    Adapter.Instance = new FakeKSPAdapter();
    manager = SpacecraftManager.Instance;
  }

  [TestMethod]
  public void Test_SimpleBatteryCharging() {
    var vessel = new FakeVessel();
    var partBattery = vessel.AddPart(vessel.rootPart);
    partBattery.AddModule(new FakeBatteryModule());
    var partRtg = vessel.AddPart(vessel.rootPart);
    partRtg.AddModule(new FakeRTGModule());

    var composite = manager.OnLoadVessel(vessel);
    var battery = partBattery.GetSimulatedComponent<Battery>(composite);
    
    composite.simulator.Simulate(5);
    // Expect 50 watts in the battery
    Assert.AreEqual(50, battery.Stored);
  }
}
