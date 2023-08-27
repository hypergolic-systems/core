using System.Diagnostics;
using System.Runtime.InteropServices;
using Hgs.Core;
using Hgs.Core.Virtual;
using Hgs.Test.FakeKSP;
using Hgs.Test.FakeMod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hgs.Test.Virtual;

[TestClass]
public class VesselShapeTests {

  private SpacecraftManager manager;

  [TestInitialize]
  public void TestInitialize() {
    Adapter.Instance = new FakeKSPAdapter();
    manager = SpacecraftManager.Instance;
  }

  [TestMethod]
  public void Test_SinglePart() {
    var vessel = new FakeVessel();
    var composite = manager.OnLoadVessel(vessel);

    Assert.AreEqual(composite.spacecraft.Count, 1);
    Assert.AreEqual(composite.spacecraft[0].segmentsByDefiningPart.Count, 1);
  }

  [TestMethod]
  public void Test_TwoPartsTwoSegments() {
    var vessel = new FakeVessel();
    var part2 = vessel.AddPart(vessel.rootPart);
    part2.AddModule(new FakeDecouplerModule());
    var composite = manager.OnLoadVessel(vessel);

    Assert.AreEqual(1, composite.spacecraft.Count);
    var craft = composite.spacecraft[0];
    Assert.AreEqual(2, craft.segmentsByDefiningPart.Count);
  }
}
