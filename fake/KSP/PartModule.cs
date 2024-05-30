using System;
using System.Collections.Generic;
using UnityEngine;

public class PartModule {
  public string moduleName;
  public Part part;
  public Vessel vessel;

  public GameObject gameObject;


  public Dictionary<string, BaseField> Fields;

  public virtual void OnAwake() {}
  public virtual void OnStart(StartState state) {}

  public virtual void OnLoad(ConfigNode node) {}
  public virtual void OnSave(ConfigNode node) {}

  [Flags]
  public enum StartState {
    Editor,
  } 
}
