using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Hgs.Core.Virtual;

public abstract class HgVirtualPartModule : PartModule, IVirtualPartModule  {
  public VirtualPart VirtualPart { get; set; }

  public VirtualVessel VirtualVessel { get; set; }

  protected bool IsInEditor = false;

  public static Dictionary<Type, List<FieldInfo>> StaticFieldsByType = new();

  public override void OnStart(StartState state) {
    base.OnStart(state);
    if (StaticFieldsByType.ContainsKey(GetType())) {
      var proto = part.partInfo.partPrefab.GetComponent(GetType());
      Debug.Assert(proto != null, $"No prototype found for {GetType().Name}");
      foreach (var field in StaticFieldsByType[GetType()]) {
        field.SetValue(this, field.GetValue(proto));
      }
    }
    VirtualPart.OnStart(this, state);
  }

  public void OnDestroy() {
    VirtualPart?.OnDestroy();
  }

  public override void OnLoad(ConfigNode node) {
    base.OnLoad(node);
    VirtualPart.OnLoad(this, node);
  }

  public override void OnSave(ConfigNode node) {
    base.OnSave(node);
    VirtualPart.OnSave(node);
  }

  public virtual void LoadStaticData(ConfigNode node) {
    var type = GetType();
    var staticDataFields = type.GetFields().Where(f => f.GetCustomAttribute<StaticField>() != null).ToList();
    if (staticDataFields.Count == 0) {
      return;
    }

    StaticFieldsByType[type] = staticDataFields;
  }

  public virtual void OnSynchronized() {}

  public abstract void InitializeComponents();
}
