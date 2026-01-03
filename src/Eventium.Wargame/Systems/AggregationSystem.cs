// <copyright file="AggregationSystem.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core;
using Eventium.Core.Systems;
using Eventium.Wargame.Components;
using Eventium.Wargame.Domain;

namespace Eventium.Wargame.Systems;

/// <summary>
/// System that processes aggregation pulse events and recalculates formation metrics.
/// </summary>
public sealed class AggregationSystem : ISystem
{
    /// <summary>
    /// Event type that triggers aggregation: AGGREGATION_PULSE.
    /// </summary>
    public IEnumerable<string> HandledEventTypes => new[] { "AGGREGATION_PULSE" };

    /// <summary>
    /// Processes an aggregation pulse event for a formation entity.
    /// </summary>
    public void HandleEvent(ISimulationContext context, Core.Events.Event evt)
    {
        var payload = evt.GetPayload<Events.AggregationPulse>();
        var entity = context.World.GetEntity(payload.FormationEntityId);

        if (entity == null)
        {
            return;
        }

        var echelon = entity.GetComponent<Echelon>("ECHELON");
        var children = entity.GetComponent<Children>("CHILDREN");
        var aggregate = entity.GetComponent<FormationAggregate>("FORMATION_AGGREGATE");

        if (echelon == null || children == null || aggregate == null)
        {
            return;
        }

        // Aggregate metrics from immediate children
        int totalStrength = 0;
        int immediateChildCount = children.ChildEntityIds.Count;

        foreach (var childId in children.ChildEntityIds)
        {
            var child = context.World.GetEntity(childId);
            if (child != null)
            {
                var childHealth = child.GetComponent<Health>("HEALTH");
                if (childHealth != null)
                {
                    totalStrength += childHealth.CurrentStrength;
                }
            }
        }

        // Update aggregate component
        aggregate.AggregatedStrength = totalStrength;
        aggregate.ImmediateChildCount = immediateChildCount;
        aggregate.LastAggregationTime = payload.PulseTime;
        aggregate.IsDirty = false;
    }
}
