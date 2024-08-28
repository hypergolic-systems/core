namespace Hgs.Core.Simulator;

public interface ISimulated {
  void RecomputeState();

  /// <summary>
  /// The maximum Δt that the current state is valid for, assuming no outside events cause an
  /// invalidation.
  /// </summary>
  ulong RemainingValidDeltaT { get; }

  void Tick(ulong deltaT);
  void OnSynchronized();

  void OnStabilized();
}
