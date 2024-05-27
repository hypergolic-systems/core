using System.Collections.Generic;

public class Part {
  public uint persistentId;
  public string name;
  public Part parent;

  public PartInfo partInfo;

  public List<Part> children;

  public List<PartModule> Modules;
}