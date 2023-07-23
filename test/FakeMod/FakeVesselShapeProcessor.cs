using Hgs.Core.Virtual;

namespace Hgs.Test.FakeMod;

public class FakeVesselShapeProcessor : VesselShapeProcessor {
  protected override void IngestPartIntoSegment(CompositeSpacecraft composite, Segment segment, object part) {
    // Do nothing for now.
  }
}