
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
public class UI_Control : Attribute {
  public UI_Scene affectSymCounterparts;
  public bool controlEnabled;
}