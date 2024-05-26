using System.Collections.Generic;

namespace Hgs.Core.Virtual;

public interface IVirtualPartModule {

  public object module { get; }

  public object gamePart { get; }

  public VirtualPart virtualPart { get; set; }

  public abstract void OnSynchronized();
}
