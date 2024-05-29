namespace Hgs.Core.Virtual;

public abstract class HgVirtualPartModule : PartModule, IVirtualPartModule  {
  public VirtualPart VirtualPart { get; set; }

  public VirtualVessel VirtualVessel { get; set; }

  protected bool IsInEditor = false;

  public override void OnStart(StartState state) {
    base.OnStart(state);
    VirtualPart.OnStart(this, state);
  }

  public void OnDestroy() {
    VirtualPart?.OnDestroy();
  }

  public override void OnLoad(ConfigNode node) {
    base.OnLoad(node);
    VirtualPart.OnLoad(this, node);
  }

  public override void OnSave(ConfigNode node) {
    base.OnSave(node);
    VirtualPart.OnSave(node);
  }

  public virtual void OnSynchronized() {}

  public abstract void InitializeComponents();
}
