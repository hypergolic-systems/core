using System;


[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class KSPField : Attribute {
    public bool guiActive;
    public bool guiActiveEditor;
    public bool isPersistant;

    public string guiName;
    public string guiUnits;
    public string groupName;
    public string groupDisplayName;

    public KSPField() {}
}