using System;
using System.Collections.Generic;
using Hgs.Core.System.Electrical;
using Hgs.Core.Virtual;

namespace Hgs.Mod.Modules;

public abstract class HgPartBase : PartModule, SimulatedModule
{

  public object module { get { return this; } }

  public object gamePart { get { return this.part; } }

  public SpacecraftPart spacecraftPart { get; set; }

  public List<VirtualComponent> virtualComponents { get; set; }

  public abstract bool OwnsComponent(VirtualComponent component);

  public abstract void InitializeComponents(SpacecraftPart part);
  public abstract void OnLinkToSpacecraft(CompositeSpacecraft sc);
  public abstract void OnSimulationUpdate(uint delta);
  public abstract void OnUnlinkFromSpacecraft(CompositeSpacecraft sc);
}
