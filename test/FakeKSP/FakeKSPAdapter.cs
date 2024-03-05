using System.Collections.Generic;
using Hgs.Core;

namespace Hgs.Test.FakeKSP;

public class FakeKSPAdapter : IAdapter
{
  public void ConfigNode_AddNode(object nodeObj, object childNodeObj) {
    throw new System.NotImplementedException();
  }

  public object ConfigNode_Create(string name) {
    throw new System.NotImplementedException();
  }

  public string ConfigNode_Get(object nodeObj, string name) {
    throw new System.NotImplementedException();
  }

  public object ConfigNode_GetNode(object nodeObj, string name) {
    throw new System.NotImplementedException();
  }


  public object[] ConfigNode_GetNodes(object nodeObj, string name) {
    throw new System.NotImplementedException();
  }

  public void ConfigNode_Set(object nodeObj, string name, string value) {
    throw new System.NotImplementedException();
  }

  public IEnumerable<object> Part_children(object part) {
    return (part as FakePart).children;
  }

  public T Part_FindModuleImplementing<T>(object part) where T : class {
    return (part as FakePart).FindModuleImplementing<T>();
  }

  public object Part_parent(object part) {
    return (part as FakePart).parent;
  }

  public uint Part_persistentId(object part) {
    return (part as FakePart).persistentId;
  }

  public object Vessel_rootPart(object vessel) {
    return (vessel as FakeVessel).rootPart;
  }

  public List<T> Vessel_FindPartModulesImplementing<T>(object vessel) where T : class {
    return (vessel as FakeVessel).FindPartModulesImplementing<T>();
  }

  uint IAdapter.Vessel_persistentId(object vessel) {
    return (vessel as FakeVessel).persistentId;
  }

  public double Game_UniversalTime() {
    return 0;
  }
}
