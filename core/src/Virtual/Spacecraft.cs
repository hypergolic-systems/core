using System.Collections.Generic;

namespace Hgs.Core.Virtual;

public class Spacecraft {
  public Composite composite;
  public uint controllingPart;
  public Dictionary<uint, Segment> segmentsByDefiningPart = new();
}
