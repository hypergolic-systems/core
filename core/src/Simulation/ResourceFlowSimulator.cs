using System;
using System.Collections.Generic;

namespace Hgs.Core.Simulation;

/// <summary>
/// Simulates flows of a specific resource within a spacecraft.
/// </summary>
public class ResourceFlowSimulator(ResourceFlowSimulator.IFlowResolver director) {

  public bool Dirty { get; internal set; } = true;

  private List<ResourceFlow> flows = new List<ResourceFlow>();

  public void Reflow() {
    if (!this.Dirty) {
      return;
    }
    director.ResolveFlows(this.flows);
  }

  public ResourceFlow Flow() {  
    var flow = new ResourceFlow(this);
    flows.Add(flow);
    return flow;
  }


  public void Tick(double deltaT) {
    if (this.Dirty) {
      throw new Exception("Cannot Tick() a dirty simulation");
    }
    foreach (var flow in this.flows) {
      flow.Tick(deltaT);
    }
  }

  public interface IFlowResolver {
    void ResolveFlows(List<ResourceFlow> flows);
  }
}