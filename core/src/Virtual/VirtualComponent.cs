
using System;
using System.Collections.Generic;
using Hgs.Core.System.Electrical.Components;

namespace Hgs.Core.Virtual;

public abstract class VirtualComponent {

  private static List<Type> COMPONENT_TYPES = new() {
    typeof(Battery),
    typeof(RadioisotopeThermalGenerator),
  };

  private static Dictionary<string, Type> COMPONENT_TYPE_MAP = new();

  static VirtualComponent() {
    foreach (var type in COMPONENT_TYPES) {
      COMPONENT_TYPE_MAP[type.Name] = type;
    }
  }

  public static VirtualComponent FromConfig(VirtualPart part, object node, bool initial) {
    var type = Adapter.ConfigNode_Get(node, "type");

    if (!COMPONENT_TYPE_MAP.ContainsKey(type)) {
      throw new Exception($"Unknown component type: {type}");
    }

    var component = (VirtualComponent)Activator.CreateInstance(COMPONENT_TYPE_MAP[type]);
    component.part = part;
    if (initial) {
      component.LoadInitial(node);
    } else {
      component.Load(node);
    }

    return component;
  }

  public object SaveConfig(bool initial) {
    var node = Adapter.ConfigNode_Create("COMPONENT");
    Adapter.ConfigNode_Set(node, "type", GetType().Name);
    if (initial) {
      SaveInitial(node);
    } else {
      Save(node);
    }
    return node;
  }

  public int index;

  public VirtualPart part;

  public IVirtualPartModule virtualModule;

  protected abstract void Save(object node);

  protected abstract void Load(object node);

  public abstract void OnActivate(VirtualVessel virtualVessel);

  protected virtual void SaveInitial(object node) {
    Save(node);
  }

  protected virtual void LoadInitial(object node) {
    Load(node);
  }
}
