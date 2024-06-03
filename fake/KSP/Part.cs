using System;
using System.Collections.Generic;
using UnityEngine;

public class Part : Component {
  public uint persistentId;
  public string name;
  public Part parent;

  public PartInfo partInfo;

  public List<Part> children;

  public List<PartModule> Modules;

  public float staticPressureAtm;

  public IEnumerable<Transform> FindModelTransforms(string transform) {
    throw new NotImplementedException();
  }

  public void AddForceAtPosition(Vector3 force, Vector3 pos) {}

  public FXGroup findFxGroup(string fxName) {
    throw new NotImplementedException();
  }

  public List<FXGroup> fxGroups;
}