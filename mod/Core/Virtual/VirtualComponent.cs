
using System;
using System.Collections.Generic;
using Hgs.Game.Components.Electrical;
using Hgs.Game.Components.Tankage;

namespace Hgs.Core.Virtual;

public abstract class VirtualComponent {

  private static List<Type> COMPONENT_TYPES = new() {
    typeof(Battery),
    typeof(RadioisotopeThermalGenerator),
    typeof(Tank),
  };

  private static Dictionary<string, Type> COMPONENT_TYPE_MAP = new();

  static VirtualComponent() {
    foreach (var type in COMPONENT_TYPES) {
      COMPONENT_TYPE_MAP[type.Name] = type;
    }
  }

  public static VirtualComponent FromConfig(VirtualPart part, ConfigNode node, bool initial) {
    var type = node.GetValue("type");

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

  public object SaveConfig(ConfigNode node, bool initial) {
    node.AddValue("type", GetType().Name);
    if (initial) {
      SaveInitial(node);
    } else {
      Save(node);
    }
    return node;
  }

  public int index;

  public VirtualPart part;

  public HgVirtualPartModule virtualModule;

  protected abstract void Save(ConfigNode node);

  protected abstract void Load(ConfigNode node);

  public abstract void OnActivate(VirtualVessel virtualVessel);

  protected virtual void SaveInitial(ConfigNode node) {
    Save(node);
  }

  protected virtual void LoadInitial(ConfigNode node) {
    Load(node);
  }
}
