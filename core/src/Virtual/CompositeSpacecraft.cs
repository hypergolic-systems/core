using System;
using System.Collections.Generic;
using Hgs.Core.System.Electrical;

namespace Hgs.Core.Virtual;

public class Segment {
  public Spacecraft craft;
  public uint definingPart;
  public Dictionary<uint, SpacecraftPart> parts = new();

  public Segment(Spacecraft craft, uint definingPart) {
    this.craft = craft;
    this.definingPart = definingPart;
  }
}

public class Spacecraft {
  public CompositeSpacecraft composite;
  public uint controllingPart;
  public Dictionary<uint, Segment> segmentsByDefiningPart = new();
}

public class SpacecraftPart {

  public uint id;

  public List<VirtualComponent> components = new();

  public void AddComponent(VirtualComponent component) {
    component.index = components.Count;
    components.Add(component);
  }
}

public class CompositeSpacecraft {
  public uint id;

  public List<Spacecraft> spacecraft = new();
  public Dictionary<uint, SpacecraftPart> partMap = new();
  public Dictionary<uint, Segment> segmentsByPart = new();
  public Segment rootSegment;

  public object liveVessel = null;

  public CompositeSpacecraft(uint vesselId) {
    this.id = vesselId;
  }

  public void ClearStructure() {
    this.spacecraft.Clear();
    this.segmentsByPart.Clear();
  }

  public void Tick(uint seconds) {}

  public void PostTick(uint seconds) {}
}

