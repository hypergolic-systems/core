using System;
using System.Collections.Generic;

namespace Hgs.Core;

public class Adapter {
  public static IAdapter Instance;
  public static string ConfigNode_Get(object nodeObj, string name) {
    return Instance.ConfigNode_Get(nodeObj, name);
  }

  public static void ConfigNode_Set(object nodeObj, string name, string value) {
    Instance.ConfigNode_Set(nodeObj, name, value);
  }

  public static object ConfigNode_GetNode(object nodeObj, string name) {
    return Instance.ConfigNode_GetNode(nodeObj, name);
  }

  public static object[] ConfigNode_GetNodes(object nodeObj, string name) {
    return Instance.ConfigNode_GetNodes(nodeObj, name);
  }

  public static object ConfigNode_Create(string name) {
    return Instance.ConfigNode_Create(name);
  }

  public static void ConfigNode_AddNode(object nodeObj, object childNodeObj) {
    Instance.ConfigNode_AddNode(nodeObj, childNodeObj);
  }
  
  public static object Vessel_rootPart(object vessel) {
    return Instance.Vessel_rootPart(vessel);
  }
  
  public static T Part_FindModuleImplementing<T>(object part) where T: class {
    return Instance.Part_FindModuleImplementing<T>(part);
  }

  public static uint Part_persistentId(object part) {
    return Instance.Part_persistentId(part);
  }

  public static object Part_parent(object part) {
    return Instance.Part_parent(part);
  }

  public static IEnumerable<object> Part_children(object part) {
    return Instance.Part_children(part);
  }

  public static List<T> Vessel_FindPartModulesImplementing<T>(object vessel) where T: class {
    return Instance.Vessel_FindPartModulesImplementing<T>(vessel);
  }

  public static uint Vessel_persistentId(object vessel) {
    return Instance.Vessel_persistentId(vessel);
  }
}

public interface IAdapter {
  string ConfigNode_Get(object nodeObj, string name);
  void ConfigNode_Set(object nodeObj, string name, string value);

  object ConfigNode_GetNode(object nodeObj, string name);

  object[] ConfigNode_GetNodes(object nodeObj, string name);

  object ConfigNode_Create(string name);

  void ConfigNode_AddNode(object nodeObj, object childNodeObj);

  object Vessel_rootPart(object vessel);

  uint Vessel_persistentId(Object vessel);

  public T Part_FindModuleImplementing<T>(object part) where T : class;

  public uint Part_persistentId(object part);

  public object Part_parent(object part);

  public IEnumerable<object> Part_children(object part);

  public List<T> Vessel_FindPartModulesImplementing<T>(object vessel) where T: class;
}
