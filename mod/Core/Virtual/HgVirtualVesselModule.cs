
using System.Collections.Generic;
using Hgs.Core.Simulator;
using UnityEngine;

namespace Hgs.Core.Virtual;

/**
  * Provides the link between a `Vessel` and its corresponding `VirtualVessel`
  * and the game version of `Vessel`s.
  */
public class HgVirtualVesselModule : VesselModule {
  /**
    * The `VirtualVessel` associated with this vessel, or `null` if none exists.
    */
  public VirtualVessel virtualVessel = new();

  public static HashSet<VirtualVessel> AllVirtualVessels = new();

  // public override Activation GetActivation() {
  //   return Activation.AllScenes;
  // }


  protected override void OnAwake() {
    base.OnAwake();
    if (vessel == null) {
      return;
    }

    virtualVessel.liveVessel = vessel;
    foreach (var resource in virtualVessel.resources.Values) {
      SimulationDriver.Instance.AddTarget(resource);
    }

    foreach (var part in virtualVessel.virtualParts.Values) {
      foreach (var component in part.Components) {
        component.OnActivate(virtualVessel);
      }
    }

    AllVirtualVessels.Add(this.virtualVessel);
  }

  protected void OnDestroy() {
    AllVirtualVessels.Remove(this.virtualVessel);
  }

  protected override void OnLoad(ConfigNode node) {
    base.OnLoad(node);
    virtualVessel?.OnLoad(node);
  }

  protected override void OnSave(ConfigNode node) {
    base.OnSave(node);
    virtualVessel.OnSave(node);
  }
}
