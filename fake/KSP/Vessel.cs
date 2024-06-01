using System;
using System.Collections.Generic;

public class Vessel {

  public FlightCtrlState ctrlState;

  public uint persistentId;
  public List<Part> parts;
  public Part rootPart;

  public List<VesselModule> vesselModules;
  public bool loaded;
  public Situation situation;

  public string name;
  public string GetDisplayName() {
    return name;
  }

  public string vesselName {
    get => name;
  }

  public VesselType vesselType;

  public enum Situation {
    LANDED,
  }
}

public enum VesselType {
  SpaceObject,
  Debris,
  Probe,
}