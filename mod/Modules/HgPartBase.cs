using Hgs.Core.Virtual;

namespace Hgs.Mod.Modules;

public abstract class HgPartBase : PartModule, VirtualModule  {

  public object module { get { return this; } }

  public object gamePart { get { return this.part; } }

  public VirtualPart virtualPart { get; set; }

  public abstract bool OwnsComponent(VirtualComponent component);

  public abstract void InitializeComponents(VirtualVessel sc, VirtualPart part);
  public virtual void OnLinkToSpacecraft(VirtualVessel sc) {}
  public virtual void OnUnlinkFromSpacecraft(VirtualVessel sc) {}
  public virtual void OnSynchronized() {}
}
