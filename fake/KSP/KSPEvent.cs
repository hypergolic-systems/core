using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class KSPEvent : Attribute {
  public bool guiActive;
  public bool guiActiveEditor;
  public string guiName;
}