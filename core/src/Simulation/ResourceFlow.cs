using System;
using System.IO;

namespace Hgs.Core.Simulation;

/// <summary>
/// A simulated interaction between a resource and a component which is either producing, consuming,
/// or storing that resource.
/// </summary>
public class ResourceFlow(ResourceSystem sim) {
  public delegate void OnSetActiveRateFn(double rate);
  public delegate void OnFlowFn(double amount);

  private double _canProduceRate = 0f;
  private double _canConsumeRate = 0f;
  private int _storageTier = 0;

  public double RemainingValidDeltaT = double.MaxValue;

  public double CanProduceRate {
    get => _canProduceRate;
    set {
      if (_canProduceRate == value) {
        return;
      }
      Console.WriteLine("[Flow] going dirty because production: " + _canProduceRate + " != " + value);
      _canProduceRate = value;
      sim.IsDirty = true;
    }
  }

  public double CanConsumeRate {
    get => _canConsumeRate;
    set {
      if (_canConsumeRate == value) {
        return;
      }
      Console.WriteLine("[Flow] going dirty because consumption: " + _canConsumeRate+ " != " + value);
      _canConsumeRate = value;
      sim.IsDirty = true;
    }
  }

  /// <summary>
  /// Storage tier represents the nature of the container which is producing/consuming this flow.
  /// Producers will not flow resources to consumers of the same or higher tier.
  /// </summary>
  public int StorageTier {
    get => _storageTier;

    set {
      if (_storageTier == value) {
        return;
      }
      Console.WriteLine("[Flow] going dirty because storage: " + _storageTier + " != " + value);
      _storageTier = value;
      sim.IsDirty = true;
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
      Console.WriteLine("[Flow] going dirty because priority" + _priority + " != " + value);
      _priority = value;
      sim.IsDirty = true;
    }
  }

  public double ActiveRate { get; internal set; } = 0f;

  public OnSetActiveRateFn OnSetActiveRate = null;
  public OnFlowFn OnFlow = null;

  internal void Tick(double deltaT) {
    OnFlow?.Invoke(this.ActiveRate * deltaT);
  }
}
