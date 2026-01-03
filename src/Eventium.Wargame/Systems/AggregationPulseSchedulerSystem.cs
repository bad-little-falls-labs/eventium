// <copyright file="AggregationPulseSchedulerSystem.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core;
using Eventium.Core.Systems;
using Eventium.Wargame.Components;

namespace Eventium.Wargame.Systems;

/// <summary>
/// System that schedules aggregation pulses at regular intervals for formations.
/// </summary>
public sealed class AggregationPulseSchedulerSystem : ISystem
{
    private readonly double _pulseInterval;

    /// <summary>
    /// Initializes a new instance of the AggregationPulseSchedulerSystem.
    /// </summary>
    /// <param name="pulseInterval">The time interval between aggregation pulses.</param>
    public AggregationPulseSchedulerSystem(double pulseInterval = 1.0)
    {
        _pulseInterval = pulseInterval;
    }

    /// <summary>
    /// Handles turn start events to schedule aggregation pulses.
    /// </summary>
    public IEnumerable<string> HandledEventTypes => new[] { "TURN_START" };

    /// <summary>
    /// Processes a turn start event and schedules aggregation pulses for all formations.
    /// </summary>
    public void HandleEvent(ISimulationContext context, Core.Events.Event evt)
    {
        var currentTime = context.Time;

        // Schedule aggregation pulses for all entities with FormationAggregate component
        foreach (var entity in context.World.Entities.Values)
        {
            var aggregate = entity.GetComponent<FormationAggregate>("FORMATION_AGGREGATE");
            if (aggregate != null)
            {
                var nextPulseTime = aggregate.LastAggregationTime + _pulseInterval;
                if (currentTime >= nextPulseTime || nextPulseTime - currentTime < 0.001)
                {
                    context.Schedule(
                        time: currentTime,
                        type: "AGGREGATION_PULSE",
                        payload: new Events.AggregationPulse(
                            FormationEntityId: entity.Id,
                            PulseTime: currentTime));
                }
            }
        }
    }
}
