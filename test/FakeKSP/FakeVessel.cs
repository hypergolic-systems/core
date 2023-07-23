using System.Collections.Generic;
using System.Linq;
using System.Runtime;

namespace Hgs.Test.FakeKSP;

public class FakeVessel {
  public List<FakePart> parts = new();
  private Dictionary<uint, FakePart> partsById = new();
  private uint nextPersistentId = 1;
  public FakePart rootPart;

  public FakeVessel() {
    this.rootPart = new FakePart(this.nextPersistentId++, null);
    this.parts.Add(rootPart);
    this.partsById.Add(this.rootPart.persistentId, this.rootPart);
  }

  public List<T> FindPartModulesImplementing<T>() where T : class {
    return parts.SelectMany(part => part.modules.OfType<T>()).ToList();
  }

  public FakePart AddPart(FakePart parent) {
    var part = new FakePart(this.nextPersistentId++, parent);
    this.parts.Add(part);
    this.partsById.Add(part.persistentId, part);
    return part;
  }
}
