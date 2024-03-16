using System.Collections.Generic;
using Hgs.Core;

namespace Hgs.Mod;

public class KspAdapter : IAdapter {
  public string ConfigNode_Get(object nodeObj, string name) {
    return (nodeObj as ConfigNode).GetValue(name);
  }

  public void ConfigNode_Set(object nodeObj, string name, string value) {
    (nodeObj as ConfigNode).SetValue(name, value, true);
  }

  public object ConfigNode_GetNode(object nodeObj, string name) {
    return (nodeObj as ConfigNode).GetNode(name);
  }

  public object[] ConfigNode_GetNodes(object nodeObj, string name) {
    return (nodeObj as ConfigNode).GetNodes(name);
  }

  public object ConfigNode_Create(string name) {
    return new ConfigNode(name);
  }

  public void ConfigNode_AddNode(object nodeObj, object childNodeObj) {
    (nodeObj as ConfigNode).AddNode(childNodeObj as ConfigNode);
  }

  public object Vessel_rootPart(object vessel) {
    return (vessel as Vessel).rootPart;
  }

  public uint Vessel_persistentId(object vessel) {
    return (vessel as Vessel).persistentId;
  }

  public T Part_FindModuleImplementing<T>(object part) where T : class {
    return (part as Part).FindModuleImplementing<T>();
  }

  public uint Part_persistentId(object part) {
    return (part as Part).persistentId;
  }

  public object Part_parent(object part) {
    return (part as Part).parent;
  }

  public IEnumerable<object> Part_children(object part) {
    return (part as Part).children;
  }

  public List<T> Vessel_FindPartModulesImplementing<T>(object vessel) where T : class {
    return (vessel as Vessel).FindPartModulesImplementing<T>();
  }

  public double Game_UniversalTime() {
    return HighLogic.CurrentGame.UniversalTime;
  }

  public void Log(string message) {
    UnityEngine.Debug.Log("[HGS] " + message);
  }
}
