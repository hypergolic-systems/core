using System;
using System.Collections.Generic;
using UnityEngine;

public class Part {
  public uint persistentId;
  public string name;
  public Part parent;

  public PartInfo partInfo;

  public List<Part> children;

  public List<PartModule> Modules;

  public IEnumerable<Transform> FindModelTransforms(string transform) {
    throw new NotImplementedException();
  }

  public FXGroup findFxGroup(string fxName) {
    throw new NotImplementedException();
  }

  public List<FXGroup> fxGroups;
}