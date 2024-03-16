using System;
using System.Collections.Generic;
using Hgs.Core.System.Electrical;
using Hgs.Core.Virtual;

namespace Hgs.Mod.Modules;

public abstract class HgPartBase : PartModule, VirtualModule  {

  public object module { get { return this; } }

  public object gamePart { get { return this.part; } }

  public VirtualPart virtualPart { get; set; }

  public List<VirtualComponent> virtualComponents { get; set; }

  public abstract bool OwnsComponent(VirtualComponent component);

  public abstract void InitializeComponents(Composite sc, VirtualPart part);
  public virtual void OnLinkToSpacecraft(Composite sc) {}
  public virtual void OnUnlinkFromSpacecraft(Composite sc) {}
  public virtual void OnSynchronized() {}
}
