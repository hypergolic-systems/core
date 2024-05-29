using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hgs.Core.Virtual;

public class VirtualPart {

  public VirtualPart() {}

  public uint id;

  public Spacecraft spacecraft;

  public IVirtualPartModule liveModule;
  public List<VirtualComponent> Components = new();

  public bool IsInEditor = false;
  protected PartModule partModule => liveModule as PartModule;

  public VirtualComponent AddComponent(VirtualComponent component) {
    component.part = this;
    component.index = Components.Count;
    Components.Add(component);
    return component;
  }

  public void OnSave(ConfigNode node) {
    if (!IsInEditor) {
      // The `VirtualVessel` infrastructure will save the state of our `VirtualComponent`(s)
      // independently of the `PartModule` config, so we don't have to worry about that.
      return;
    }

    node.AddValue("initial", true);
    foreach (var component in Components) {
      component.SaveConfig(node.AddNode("COMPONENT"), /* initial */ true);
    }
  }

  public static void OnLoad(IVirtualPartModule module, ConfigNode node) {
    var partModule = module as PartModule;
    if (partModule == null) {
      throw new Exception($"VirtualPart.OnStart: IVirtualPartModule {module.GetType().FullName} is not a PartModule");
    }
  
    if (!node.HasValue("initial")) {
      return;
    }

    var virtualPart = module.VirtualPart = new VirtualPart() {
      id = partModule.part.persistentId,
      liveModule = module,
    };

    foreach (var componentNode in node.GetNodes("COMPONENT")) {
      virtualPart.AddComponent(VirtualComponent.FromConfig(virtualPart, componentNode, /* initial */ true));
    }
  }

  public static void OnStart(IVirtualPartModule module, PartModule.StartState state) {
    var partModule = module as PartModule;
    if (partModule == null) {
      throw new Exception($"VirtualPart.OnStart: IVirtualPartModule {module.GetType().FullName} is not a PartModule");
    }

    if (state.HasFlag(PartModule.StartState.Editor)) {
      if (module.VirtualPart == null) {
        // This is the first time the part is being initialized in the editor, so it needs to be
        // initialized as a completely fresh part.
        module.VirtualPart = new VirtualPart() {
          id = partModule.part.persistentId,
          liveModule = module,
          IsInEditor = true,
        };
        module.InitializeComponents();
      } else {
        module.VirtualPart.IsInEditor = true;
      }
    } else {
      var virtualPart = module.VirtualPart;
      var virtualVessel = module.VirtualVessel = partModule.vessel.vesselModules.OfType<HgVirtualVesselModule>().FirstOrDefault().virtualVessel;
      if (virtualPart != null) {
        virtualVessel.virtualParts[partModule.part.persistentId] = virtualPart;
      } else {
        virtualPart = module.VirtualPart = virtualVessel.virtualParts[partModule.part.persistentId];
        virtualPart.liveModule = module;
      }
    }
  }

  public void OnDestroy() {
    liveModule = null;
  }
}
