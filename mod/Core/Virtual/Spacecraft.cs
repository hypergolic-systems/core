using System.Collections.Generic;

namespace Hgs.Core.Virtual;

public class Spacecraft {
  public VirtualVessel virtualVessel;
  public uint controllingPart;
  public Dictionary<uint, Segment> segmentsByDefiningPart = new();
}
