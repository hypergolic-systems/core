using System.Collections.Generic;

namespace Hgs.Core.Virtual;

public class VirtualPart {

  public uint id;

  public Spacecraft spacecraft;

  public IVirtualPartModule virtualModule;

  public List<VirtualComponent> components = new();

  public VirtualComponent AddComponent(VirtualComponent component) {
    component.part = this;
    component.index = components.Count;
    components.Add(component);
    return component;
  }
}
