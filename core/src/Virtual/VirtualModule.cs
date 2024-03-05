using Hgs.Core.Virtual;
using System;
using System.Collections.Generic;

namespace Hgs.Core.Virtual;

public interface VirtualModule {

  public object module { get; }

  public object gamePart { get; }

  public VirtualPart virtualPart { get; set; }

  public abstract bool OwnsComponent(VirtualComponent component);
  public abstract void OnLinkToSpacecraft(Composite sc);

  public abstract void OnUnlinkFromSpacecraft(Composite sc);

  public abstract void InitializeComponents(Composite sc, VirtualPart part);

  public abstract void OnSynchronized();
}
