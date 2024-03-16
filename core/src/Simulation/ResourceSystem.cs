using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Hgs.Core.Simulation;

/// <summary>
/// Simulates flows of a specific resource within a spacecraft.
/// </summary>
public class ResourceSystem(ResourceSystem.IFlowResolver director) : ISimulated {

  public bool IsDirty { get; internal set; } = true;

  private List<ResourceFlow> flows = new List<ResourceFlow>();

  public double RemainingValidDeltaT {
    get => flows.Count == 0 ? double.MaxValue : flows.Select(f => f.RemainingValidDeltaT).Min();
  }

  public string DirectorName {
    get => director.GetType().Name;
  }

  public void RecomputeState() {
    if (!this.IsDirty) {
      return;
    }
    this.IsDirty = false;
    director.ResolveFlows(this.flows);
  }

  public ResourceFlow NewFlow() {  
    var flow = new ResourceFlow(this);
    flows.Add(flow);
    return flow;
  }


  public void Tick(double deltaT) {
    if (this.IsDirty) {
      throw new Exception("Cannot Tick() a dirty simulation");
    }
    foreach (var flow in this.flows) {
      flow.Tick(deltaT);
    }
  }

  public void OnSynchronized() {
    foreach (var flow in this.flows) {
      if (flow.OnSynchronized != null) {
        flow.OnSynchronized();
      }
    }
  }

  public interface IFlowResolver {
    void ResolveFlows(List<ResourceFlow> flows);
  }
}