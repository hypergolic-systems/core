using System;
using System.Collections.Generic;
using System.Linq;
using Hgs.Core.Engine;
using Hgs.Core.Virtual;

namespace Hgs.Core.Resources;

/// <summary
/// A resource consumer that can handle instantaneous changes in rate via an internal buffer, while
/// processing multiple resources being consumed.
/// </summary>
///
/// <remarks>
/// Many components consume resources at a rate that the user can control on a per-frame
/// basis. The resource system is designed to simulate resource usage on a less granular
/// scale, and has a minimum tick rate of 1 Hz. In order to allow a usage rate that
/// updates more frequently than this, a buffer is used to satisfy instantaneous increases
/// in demand until the resource simulation can catch up.
/// </remarks>
/// 
public class BufferedRealtimeConsumer {

  private Dictionary<Resource, BufferedResource> resources = new();

  private float currentLiveRateFraction = 0;

  private State state = State.Startup;

  public BufferedRealtimeConsumer() {}

  public static BufferedRealtimeConsumer FromPropellantRecipe(VirtualVessel vessel, VirtualComponent engine, PropellantRecipe recipe, float maxVolumetricFlow) {
    
    var group = new BufferedRealtimeConsumer();

    var recipeTotalVolume = recipe.Ingredients.Sum(i => i.VolumePartInRecipe);
    var scalingFactor = maxVolumetricFlow / recipeTotalVolume;
    foreach (var ingredient in recipe.Ingredients) {
      var system = vessel.resources[ingredient.Resource];
      var ticket = system.NewTicket();
      ticket.Owner = engine;
      ticket.Request = 0;
      BufferedResource buffered = new BufferedResource {
        Resource = ingredient.Resource,
        System = system,
        Ticket = ticket,
        MaxRate = ingredient.VolumePartInRecipe * scalingFactor,
        Amount = 0,
      };
      group.resources[ingredient.Resource] = buffered;
      ticket.OnCommit += () => group.OnResourceCommit(buffered);
      ticket.OnTick += (deltaTime) => group.OnResourceTick(buffered, deltaTime);
    }
    return group;
  }

  public bool TryConsumeDuringFixedUpdate(float liveRateFraction, float deltaTime) {
    if (state == State.Starved) {
      // We've run out of at least one resource, and cannot satisfy requests. Requests have
      // already been set, and we're waiting on the simulation to deliver more resources.
      //
      // The only way this is successful is if we're not trying to consume anything at the moment.
      return liveRateFraction == 0;
    }

    if (deltaTime > 0) {
      float timeFactor = liveRateFraction * deltaTime;
      // Firstly, we check whether we can actually supply the requested resources.
      foreach (var buffer in resources.Values) {
        var requested = buffer.MaxRate * timeFactor;
        if (buffer.Amount < requested) {
          // We've run out of this resource, so we can't supply the requested amount.
          transitionToStarved();
          return false;
        }
      }
    }

    if (currentLiveRateFraction != liveRateFraction || state == State.Startup) {
      // The rate of consumption has changed, and we need to re-evaluate the buffer.
      currentLiveRateFraction = liveRateFraction;
      transitionToUnstable();
    } else {
      currentLiveRateFraction = liveRateFraction;
    }

    if (deltaTime > 0 && currentLiveRateFraction > 0) {
      float timeFactor = currentLiveRateFraction * deltaTime;
      foreach (var buffer in resources.Values) {
        buffer.Amount -= buffer.MaxRate * timeFactor;
      }
    }

    return true;
  }

  private void transitionToStarved() {
    // We've run out of at least one resource, and cannot satisfy requests.
    state = State.Starved;
    foreach (var buffer in resources.Values) {
      if (buffer.Amount < buffer.MaxRate) {
        // The buffer isn't full, so request a rate that will fill it within the next tick.
        buffer.Ticket.Request = buffer.MaxRate - buffer.Amount;
      } else if (buffer.Ticket.Request > 0) {
        // The buffer is full, so we don't need to fill it any more.
        buffer.Ticket.Request = 0;
      }
      buffer.Ticket.RemainingValidDeltaT = 0;
    }
  }

  private void transitionToUnstable() {
    state = State.Unstable;
    foreach (var buffer in resources.Values) {
      if (buffer.Amount >= buffer.MaxRate) {
        // We have enough resources in this buffer, so request only the new rate.
        buffer.Ticket.Request = buffer.MaxRate * currentLiveRateFraction;
      } else {
        // We need this resource to catch up in the next tick.
        buffer.Ticket.Request = buffer.MaxRate * (1 + currentLiveRateFraction);
      }
    } 
  }

  private void OnResourceTick(BufferedResource resource, ulong deltaTime) {
    // Buffer the resource according to its rate.
    resource.Amount += resource.Ticket.Rate * deltaTime;

    if (state == State.Stable) {
      // If we're in a stable state, we're guaranteed to never drop resource buffers below their
      // maximum rate, so we can skip the below logic.
      return;
    }

    var consumeRate = currentLiveRateFraction * resource.MaxRate;

    if (resource.Amount >= resource.MaxRate && resource.Ticket.Request != consumeRate) {
      // The tank has filled up.
      resource.Ticket.Request = consumeRate;
    }
  }

  private void OnResourceCommit(BufferedResource resource) {
    var ticket = resource.Ticket;
    var consumeRate = currentLiveRateFraction * resource.MaxRate;
    var gainRate = resource.Ticket.Rate - consumeRate;
    if (gainRate == 0) {
      // The buffer is not changing, so this condition is stable.
      ticket.RemainingValidDeltaT = ulong.MaxValue;
    } else if (gainRate > 0) {
      // The buffer is slowly filling.
      ticket.RemainingValidDeltaT = (ulong) Math.Ceiling((resource.MaxRate - resource.Amount) / gainRate);
    } else {
      ticket.RemainingValidDeltaT = (ulong) Math.Floor(resource.Amount / -gainRate);
    }

    var isStable = resources.Values.All(resource => resource.Amount >= resource.MaxRate && resource.Ticket.Rate == resource.Ticket.Request);
    if (isStable) {
      state = State.Stable;
    }
  }

  public float GetAmountInBuffer(Resource resource) {
    return resources[resource].Amount;
  } 

  private class BufferedResource {
    public Resource Resource;
    public ResourceSystem System;

    public ResourceSystem.Ticket Ticket;

    /// <summary>
    /// The maximum rate at which this resource can be consumed.
    /// </summary>
    public float MaxRate;

    public float Amount;
  }

  private enum State {

    // We haven't done any calculations yet.
    Startup,

    // The rate of consumption has been stable, and we are replenishing buffers at the same rate.
    Stable,

    // The rate of consumption has changed, and the buffer is currently out of sync with the
    // consumption rate (either gaining or losing). We need to adjust the consumption rate once
    // the buffers reach capacity.
    Unstable,

    // We've run out of at least one resource, and cannot satisfy requests.
    Starved,
  }
}
