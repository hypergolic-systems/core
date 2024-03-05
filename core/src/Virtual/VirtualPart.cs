using System.Collections.Generic;

namespace Hgs.Core.Virtual;

public class VirtualPart {

  public uint id;

  public List<VirtualComponent> components = new();

  public void AddComponent(VirtualComponent component) {
    component.index = components.Count;
    components.Add(component);
  }
}
