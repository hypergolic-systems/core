using System.IO;

namespace Hgs.Core.Simulation;

/// <summary>
/// A simulated interaction between a resource and a component which is either producing, consuming,
/// or storing that resource.
/// </summary>
public class ResourceFlow(ResourceFlowSimulator sim) {
  public delegate void OnSetActiveRateFn(double rate);
  public delegate void OnFlowFn(double amount);

  private double _canProduceRate = 0f;
  private double _canConsumeRate = 0f;
  private int _storageTier = 0;

  public double CanProduceRate {
    get => _canProduceRate;
    set {
      _canProduceRate = value;
      sim.Dirty = true;
    }
  }

  public double CanConsumeRate {
    get => _canConsumeRate;
    set {
      _canConsumeRate = value;
      sim.Dirty = true;
    }
  }

  /// <summary>
  /// Storage tier represents the nature of the container which is producing/consuming this flow.
  /// Producers will not flow resources to consumers of the same or higher tier.
  /// </summary>
  public int StorageTier {
    get => _storageTier;

    set {
      _storageTier = value;
      sim.Dirty = true;
    }
  }


  int _priority = 0;

  /// <summary>
  /// Priority of this flow relative to others. Higher priority consumers will be fully satisfied
  /// before lower priority consumers, and higher priority producers will be fully drained before
  /// lower priority producers.
  /// </summary>
  public int Priority {
    get => _priority;
    set {
      _priority = value;
      sim.Dirty = true;
    }
  }

  public double ActiveRate { get; internal set; } = 0f;

  public OnSetActiveRateFn OnSetActiveRate = null;
  public OnFlowFn OnFlow = null;

  internal void Tick(double deltaT) {
    OnFlow?.Invoke(this.ActiveRate * deltaT);
  }
}
