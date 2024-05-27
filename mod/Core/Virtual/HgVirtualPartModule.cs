using System.Linq;

namespace Hgs.Core.Virtual;

public abstract class HgVirtualPartModule : PartModule  {
  public VirtualPart virtualPart { get; set; }

  protected VirtualVessel virtualVessel;

  protected bool IsInEditor = false;

  public override void OnStart(StartState state) {
    base.OnStart(state);
    if (state.HasFlag(StartState.Editor)) {
      IsInEditor = true;

      if (virtualPart == null) {
        // This is the first time the part is being initialized in the editor, so it needs to be
        // initialized as a completely fresh part.
        virtualPart = new VirtualPart {
          id = part.persistentId,
          module = this,
        };
        InitializeComponents();
      }
    } else {
      virtualVessel = vessel.vesselModules.OfType<HgVirtualVesselModule>().FirstOrDefault().virtualVessel;
      if (virtualPart != null) {
        virtualVessel.virtualParts[part.persistentId] = virtualPart;
      } else {
        virtualPart = this.virtualVessel.virtualParts[part.persistentId];
        virtualPart.module = this;
      }
    }
  }

  public void OnDestroy() {
    if (virtualPart == null) {
      return;
    }
    virtualPart.module = null;
  }

  public override void OnLoad(ConfigNode node) {
    if (!node.HasValue("initial")) {
      return;
    }

    // Happens either in the editor, or when the vessel is loaded for the first time.
    // In either case, the `VirtualPart` should not exist, because it will not have been added
    // to a `VirtualVessel` configuration yet.
    this.virtualPart = new VirtualPart {
      id = part.persistentId,
      module = this,
    };
    foreach (var componentNode in node.GetNodes("COMPONENT")) {
      virtualPart.AddComponent(VirtualComponent.FromConfig(virtualPart, componentNode, /* initial */ true));
    }
  }

  public override void OnSave(ConfigNode node) {
    base.OnSave(node);
    if (!IsInEditor) {
      // The `VirtualVessel` infrastructure will save the state of our `VirtualComponent`(s)
      // independently of the `PartModule` config, so we don't have to worry about that.
      return;
    }

    node.AddValue("initial", true);
    foreach (var component in virtualPart.components) {
      component.SaveConfig(node.AddNode("COMPONENT"), /* initial */ true);
    }
  }

  public virtual void OnSynchronized() {}

  protected abstract void InitializeComponents();
}
