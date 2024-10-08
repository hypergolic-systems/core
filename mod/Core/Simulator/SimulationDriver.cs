
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Hgs.Core.Virtual;

namespace Hgs.Core.Simulator;

public class SimulationDriver {

  private const double EPSILON_TIME = 1e-6;
  private HashSet<ISimulated> targets = new HashSet<ISimulated>();

  private ulong LastSync = 0;

  public static SimulationDriver Instance;

  public static void Initialize() {
    Instance = new SimulationDriver();
  }
  
  public void AddTarget(ISimulated target) {
    targets.Add(target);
  }

  public void RemoveTarget(ISimulated target) {
    targets.Remove(target);
  }

  public void Sync(double timeDouble) {
    var time = (ulong) timeDouble;
    if (LastSync == 0) {
      // This is our first synchronization, so capture the current
      // time only. No recomputation is needed.
      LastSync = time;
      return;
    }

    // Calculate `deltaT`.
    var deltaT = time - LastSync;
    if (deltaT == 0) {
      return;
    }

    Debug.Assert(deltaT > 0);

    // Advance time forward.
    this.runTimeForward(deltaT);
    this.LastSync = time;

    foreach (var target in targets) {
      target.OnSynchronized();
    }

    var count = 0;
    foreach (var vessel in HgVirtualVesselModule.AllVirtualVessels) {
      foreach (var part in vessel.virtualParts.Values) {
        part.liveModule?.OnSynchronized();
        count++;
      }
    }
  }


  /// <summary>
  /// Simulate time forward by `deltaT` seconds.
  /// </summary>
  private void runTimeForward(ulong deltaT) {
    // It may take multiple simulation steps to progress by `deltaT`.
    while (deltaT > 0) {
      // Begin by ensuring that our simulation is up to date.
      stabilizeSimulationIfNeeded();

      // Find the largest time step that can be taken. This could be
      // smaller than `deltaT` if we're constrained by a target needing
      // multiple rounds of updates.
      var deltaTStep = deltaT;
      if (targets.Count > 0) {
        var deltaTMin = targets.Min(t => t.RemainingValidDeltaT);
        deltaTStep = Math.Min(deltaT, deltaTMin);
      }

      // Sanity check our forward step.
      Debug.Assert(deltaTStep > 0);
      Debug.Assert(deltaTStep <= deltaT);

      foreach (var target in targets) {
        target.Tick(deltaTStep);
      }

      deltaT -= deltaTStep;
    }

    stabilizeSimulationIfNeeded();
  }

  /// <summary>
  /// Get each simulated system to a stable state, ready for time to step
  /// forward.
  /// </summary>
  private void stabilizeSimulationIfNeeded() {
    // It might take several iterations for systems to stabilize, as one
    // system's stabilization may destabilize another.
    while (true) {
      // Check if any unstable targets exist.
      var dirtyTargets = targets.Where(t => t.RemainingValidDeltaT == 0).ToList();
      if (dirtyTargets.Count == 0) {
        // All targets are stable.
        return;
      }

      // Recomputing state is potentially expensive, so it's parallelized.
      // We track how many recomputations are outstanding in this round in
      // order to await their completion.
      var outstandingRecomputations = new CountdownEvent(dirtyTargets.Count);

      foreach (var target in dirtyTargets) {
        ThreadPool.QueueUserWorkItem(_ => {
          try {
            target.RecomputeState();
          } finally {
            outstandingRecomputations.Signal();
          }
        });
      }

      // Wait for all recomputation to finish. Note that some of the individual
      // simulations may have marked others dirty again.
      outstandingRecomputations.Wait();

      foreach (var target in dirtyTargets) {
        target.OnStabilized();
      }
    }
  }
}