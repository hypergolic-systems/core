using System.Collections.Generic;
using System.Linq;

namespace Hgs.Test.FakeKSP;

public class FakePart {
  public uint persistentId;
  public FakePart parent;
  public List<FakePart> children = new();
  public List<FakePartModule> modules = new();

  public FakePart(uint persistentId, FakePart parent) {
    this.persistentId = persistentId;
    this.parent = parent;
    parent?.children.Add(this);
  }

  public T FindModuleImplementing<T>() where T : class {
    return modules.OfType<T>().FirstOrDefault();
  }

  public List<T> FindModulesImplementing<T>() where T : class {
    return modules.OfType<T>().ToList();
  }

  public void AddModule(FakePartModule module) {
    module.part = this;
    modules.Add(module);
  }
}
