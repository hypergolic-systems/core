
using System;

namespace Hgs.Core.Virtual;

public abstract class VirtualComponent {
  public uint partId;
  public int index;

  public VirtualPart part;

  public VirtualModule virtualModule;

  public virtual void Save(object node) {}

  public virtual void Load(object node) {}

  public virtual void OnAttached(Composite composite) {}
}
