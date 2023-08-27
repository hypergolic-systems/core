using Hgs.Core.Virtual;
using System;
using System.Collections.Generic;

namespace Hgs.Core.Virtual;

public interface SimulatedModule {

  public object module { get; }

  public object gamePart { get; }

  public SpacecraftPart spacecraftPart { get; set; }

  public abstract bool OwnsComponent(VirtualComponent component);
  public abstract void OnLinkToSpacecraft(CompositeSpacecraft sc);

  public abstract void OnUnlinkFromSpacecraft(CompositeSpacecraft sc);

  public abstract void OnSimulationUpdate(uint delta);

  public abstract void InitializeComponents(SpacecraftPart part);
}
