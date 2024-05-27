using System;


[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class KSPField : Attribute {
    public bool guiActive;
    public bool guiActiveEditor;
    public bool isPersistant;

    public KSPField() {}
}