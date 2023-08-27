using Hgs.Core.Virtual;
using Hgs.Test.FakeKSP;

namespace Hgs.Test.FakeMod;

public class FakeDecouplerModule : FakePartModule, SegmentDivider {
  public object Part => this.part;

  public object ForeignPart => this.part.parent;
}
