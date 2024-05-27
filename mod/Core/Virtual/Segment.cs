using System.Collections.Generic;

namespace Hgs.Core.Virtual;

public class Segment {
  public Spacecraft craft;
  public uint definingPart;
  public Dictionary<uint, VirtualPart> parts = new();

  public Segment(Spacecraft craft, uint definingPart) {
    this.craft = craft;
    this.definingPart = definingPart;
  }
}