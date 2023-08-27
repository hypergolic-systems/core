
using System;

namespace Hgs.Core.Virtual;

public abstract class VirtualComponent {
  public uint partId;
  public int index;

  public SimulatedModule liveModule;

  public virtual void Save(object node) {}

  public virtual void Load(object node) {}
}
