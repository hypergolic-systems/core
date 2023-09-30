using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Hgs.Core.Simulation;

/// <summary>
/// Coordinates simulation of all of the `CompositeSpacecraft` in the universe.
/// </summary>
public class SimulationDriver {
  private SimulationThread sim = SimulationThread.Launch();

  private double upperBoundOfTime = 0;

  public SimulationDriver(double initialTime) {
    upperBoundOfTime = initialTime;
  }

  /// <summary>
  /// Block until the simulation reaches its upper time bound.
  /// </summary>
  public void Sync() {
    var ev = new ManualResetEventSlim();
    sim.actions.Add(new SyncAction { Event = ev });
    ev.Wait();
  }

  /// <summary>
  /// Allow the simulation to progress up to the given time.
  /// 
  /// Note that the simulation is asynchronous, and this method will return before the simulation
  /// has actually reached the given time. To wait for the simulation to reach the given time,
  /// follow this method with a call to `Sync()`.
  /// </summary>
  public void RaiseUpperBoundOfTime(double time) {
    var deltaT = time - upperBoundOfTime;
    Debug.Assert(deltaT >= 0);
    upperBoundOfTime = time;
    sim.actions.Add(new AdvanceTimeAction { DeltaT = deltaT });
  }

  public void AddTarget(ISimulated target) {
    sim.actions.Add(new AddTargetAction { Target = target });
  }

  public void Shutdown() {
    sim.Shutdown();
  }

  class SimulationThread {
    public BlockingCollection<SimAction> actions = new BlockingCollection<SimAction>();
    public AutoResetEvent actionAvailable = new AutoResetEvent(false);
    private HashSet<ISimulated> targets = new HashSet<ISimulated>();
    private Thread thread = null;

    public static SimulationThread Launch() {
      var sim = new SimulationThread();
      sim.thread = new Thread(sim.Run) {
        Name = "[HGS] Simulation Thread",
        IsBackground = true,
      };
      sim.thread.Start();
      return sim;
    }

    public void Shutdown() {
      actions.CompleteAdding();
      thread.Join();
    }

    private void Run() {
      double pendingDeltaT = 0;

      foreach (var action in actions.GetConsumingEnumerable()) {
        if (action is AdvanceTimeAction advance) {
          pendingDeltaT += advance.DeltaT;
          if (actions.Count > 0) {
            // We want to coalesce multiple AdvanceTimeActions in order to take as few simulation
            // steps as possible. If there are more actions to process, we simply record the
            // cumulative Δt and continue processing.
            continue;
          }
        }

        // Before executing any actions, we need to bring the simulation up to the current time.
        if (pendingDeltaT > 0) {
          tickSimulation(pendingDeltaT);
          pendingDeltaT = 0;
        }

        if (action is SyncAction sync) {
          // A SyncAction is a request to notify the caller when the simulation has reached this
          // point (all prior actions have been processed).
          sync.Event.Set();
        } else if (action is AddTargetAction add) {
          targets.Add(add.Target);
        }
      }
    }

    private void tickSimulation(double deltaT) {
      // Ticking may require multiple steps, each of a smaller Δt than the overall goal.
      while (deltaT > 0) {
        ensureValidSimulationState();

        var deltaTConstraint = targets.Min(t => t.RemainingValidDeltaT);
        var deltaTStep = Math.Min(deltaT, deltaTConstraint);
        Debug.Assert(deltaTStep > 0);
        Debug.Assert(deltaTStep <= deltaT);

        // Now that we know how long it's valid to tick the simulation for, tick all of the
        // individual simulations. This operation is done in a single-threaded manner, since ticking
        // a simulation should be a very fast operation.
        foreach (var target in targets) {
          target.Tick(deltaTStep);
        }

        deltaT -= deltaTStep;
        if (Math.Abs(deltaT) < 1e-6) {
          deltaT = 0;
        }
      }
    }

    /// <summary>
    /// Ensure that every individual simulation is in a valid state to be advanced/ticked.
    ///
    /// This might involve recomputing the state of the simulation, perhaps more than once.
    /// </summary>
    private void ensureValidSimulationState() {
      // It might take several iterations of recomputation before every simulation is valid.
      while (true) {
        var dirtyTargets = targets.Where(t => t.IsDirty).ToList();
        if (dirtyTargets.Count == 0) {
          return;
        }

        // Unlike the simple tick/advance operation, recomputing state is potentially expensive.
        // Therefore, we push the work of recomputation into a thread pool to parallelize it as
        // much as possible.

        // Track how many recomputation operations are in progress, so that we can wait for them all
        // to finish later.
        var recomputing = new CountdownEvent(dirtyTargets.Count);

        foreach (var target in dirtyTargets) {
          ThreadPool.QueueUserWorkItem(_ => {
            try {
              target.RecomputeState();
            } finally {
              recomputing.Signal();
            }
          });
        }

        // Wait for all recomputation to finish. Note that some of the individual simulations may
        // have marked others dirty again.
        recomputing.Wait();
      }
    }
  }

  abstract record SimAction {}

  record AdvanceTimeAction : SimAction {
    public double DeltaT;
  }

  record SyncAction : SimAction {
    public ManualResetEventSlim Event;
  }

  record AddTargetAction : SimAction {
    public ISimulated Target;
  }
}
