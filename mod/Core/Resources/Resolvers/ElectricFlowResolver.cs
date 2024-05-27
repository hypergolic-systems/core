using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Hgs.Core.Resources.Resolvers;


public class ElectricFlowResolver : ResourceSystem.IFlowResolver {

  public void ResolveFlows(List<ResourceFlow> flows) {
    // Positive rates are consumption, negative rates are production.
    Dictionary<ResourceFlow, double> calculatedRates = new Dictionary<ResourceFlow, double>();
    IEnumerable<ResourceFlow> loads = flows.Where(f => f.CanConsumeRate > 0);

    foreach (var flow in flows) {
      calculatedRates.Add(flow, 0);
    }

    var tiers = flows
      .Select(f => f.StorageTier)
      .Where(t => t > 0)
      .Distinct()
      .OrderBy(t => t);

    this.calculateFlowsWithinTier(calculatedRates, flows.Where(f => f.CanProduceRate > 0 && f.StorageTier == 0), loads);

    foreach (var tier in tiers) {
      this.calculateFlowsWithinTier(calculatedRates, flows.Where(f => f.CanProduceRate > 0 && f.StorageTier == tier), loads.Where(l => l.StorageTier < tier));
    }

    // Update the flows with their new rates.
    foreach (var flow in flows) {
      flow.ActiveRate = calculatedRates[flow];
      if (flow.OnSetActiveRate != null) {
        flow.OnSetActiveRate(flow.ActiveRate);
      }
    }

    Debug.Assert(flows.Sum(f => f.ActiveRate) == 0, "Net flow should be 0.");
  }

  private void calculateFlowsWithinTier(Dictionary<ResourceFlow, double> calculatedRates, IEnumerable<ResourceFlow> producers, IEnumerable<ResourceFlow> consumers) {
    // We process producers 
    foreach (var producerGroup in producers.GroupBy(f => f.Priority)) {
      // First, filter the producers down to those which are not yet fully drained.
      var liveProducers = new HashSet<ResourceFlow>(producerGroup.Where(f => calculatedRates[f] > -f.CanProduceRate));

      foreach (var consumerGroup in consumers.GroupBy(f => f.Priority)) {
        var liveConsumers = new HashSet<ResourceFlow>(consumerGroup.Where(f => calculatedRates[f] < f.CanConsumeRate));

        while (liveConsumers.Count > 0 && liveProducers.Count > 0) {

          // Next, find the smallest amount that a producer can produce.
          // Note: we add `calculatedRate` here since the production rates are negative.
          var minSingleProducer = liveProducers.Min(f => f.CanProduceRate + calculatedRates[f]);

          // And do the same for consumers and consumption:
          var minSingleConsumer = liveConsumers.Min(f => f.CanConsumeRate - calculatedRates[f]);

          // Now, we need to determine: if every producer produced the minimum and that was divided
          // among all the consumers, would it exceed the minimum consumer? If so, then we're bounded
          // by the smallest consumer. If not, then we're bounded by the smallest producer.
          var perProducer = minSingleProducer;
          if (minSingleProducer * liveProducers.Count > minSingleConsumer * liveConsumers.Count) {
            // Producing up to the capability of the smallest producer would exceed the smallest
            // consumer, so go the other way around.
            perProducer = minSingleConsumer;
            Debug.Assert(minSingleConsumer < minSingleProducer,
                "Producing the minimum producer was too much, but minimum consumer is greater");
          }

          var totalProduction = perProducer * liveProducers.Count;
          var perConsumer = totalProduction / liveConsumers.Count;

          Debug.Assert(totalProduction == perConsumer * liveConsumers.Count,
              "Production should match consumption");

          // Allocate rates.
          foreach (var producer in liveProducers) {
            calculatedRates[producer] -= perProducer;
          }
          foreach (var consumer in liveConsumers) {
            calculatedRates[consumer] += perConsumer;
          }

          // Adjust our live sets and remove saturated producers and consumers.
          liveProducers.RemoveWhere(f => calculatedRates[f] <= -f.CanProduceRate);
          liveConsumers.RemoveWhere(f => calculatedRates[f] >= f.CanConsumeRate);
        }

        // If we run out of producers, we won't be able to satisfy any more consumer demand and
        // need to move on to the next producer group.
        if (liveProducers.Count == 0) {
          break;
        }
      }
    }
  }
}