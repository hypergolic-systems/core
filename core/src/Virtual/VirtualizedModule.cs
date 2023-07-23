using Hgs.Core.Virtual;
using System.Collections.Generic;

namespace Hgs.Core.Virtual;

  /**
  * A `PartModule` which connects with one or more `VirtualPart`s.
  */
public interface VirtualizedModule {

  public object module { get; }

  public List<VirtualPart> virtualParts { get; set; }

  public abstract void OnLinkToSpacecraft(CompositeSpacecraft sc);

  public abstract void OnUnlinkFromSpacecraft(CompositeSpacecraft sc);

  public abstract void OnSimulationUpdate(uint delta);

  public abstract void InitializeVirtualParts();
}

public interface SegmentDivider {

  public object Part { get; }
  public object OtherSide { get; }
  public string DividerStyle { get; }
}
