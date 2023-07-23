using Hgs.Core;
using Hgs.Test.FakeKSP;
using Hgs.Test.FakeMod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hgs.Test.Virtual;

[TestClass]
public class VesselShapeTests {

  [TestInitialize]
  public void TestInitialize() {
    Adapter.Instance = new FakeKSPAdapter();
  }

  [TestMethod]
  public void Test_SinglePart() {
    var vessel = new FakeVessel();
    var proc = new FakeVesselShapeProcessor();
    var composite = proc.NewCompositeFromParts(vessel);

    Assert.AreEqual(composite.spacecraft.Count, 1);
    Assert.AreEqual(composite.spacecraft[0].segments.Count, 1);
    Assert.AreEqual(composite.links.Count, 0);
  }
}