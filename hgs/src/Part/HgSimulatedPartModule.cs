using Hgs.Virtual;

using System.Collections.Generic;

namespace Hgs.Part {

  /**
  * A `PartModule` which connects with one or more `VirtualPart`s.
  */
  public interface VirtualizedModule {

    public PartModule module { get; }

    public List<VirtualPart> virtualParts { get; set; }

    public abstract void OnLinkToSpacecraft(VirtualVessel sc);

    public abstract void OnUnlinkFromSpacecraft(VirtualVessel sc);

    public abstract void OnSimulationUpdate(uint delta);

    public abstract void InitializeVirtualParts();
  }
}
