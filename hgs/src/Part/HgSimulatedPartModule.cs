using System;
using Hgs.System;

namespace Hgs.Part {

  public abstract class HgSimulatedPartModule : PartModule {

    public SimulatedPart simPart = null;

    public abstract SimulatedPart CreateSimulatedPart();

    public virtual void OnLinkToSpacecraft(Spacecraft sc) {}

    public virtual void OnUnlinkFromSpacecraft(Spacecraft sc) {}

    public virtual void OnSimulationUpdate(uint delta) {}
  }
}