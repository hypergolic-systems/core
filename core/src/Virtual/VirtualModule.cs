namespace Hgs.Core.Virtual;

public interface VirtualModule {

  public object module { get; }

  public object gamePart { get; }

  public VirtualPart virtualPart { get; set; }

  public abstract bool OwnsComponent(VirtualComponent component);
  public abstract void OnLinkToSpacecraft(VirtualVessel sc);

  public abstract void OnUnlinkFromSpacecraft(VirtualVessel sc);

  public abstract void InitializeComponents(VirtualVessel sc, VirtualPart part);

  public abstract void OnSynchronized();
}
