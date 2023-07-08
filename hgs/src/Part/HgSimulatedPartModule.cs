using Hgs.Virtual;

using System.Collections.Generic;

namespace Hgs.Part {

  public interface IVirtualizedModule {

    public PartModule module { get; }

    public List<Virtual.VirtualPart> virtualParts { get; set; }

    public abstract void OnLinkToSpacecraft(VirtualVessel sc);

    public abstract void OnUnlinkFromSpacecraft(VirtualVessel sc);

    public abstract void OnSimulationUpdate(uint delta);

    public abstract void InitializeVirtualParts();
  }
}
