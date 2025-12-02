// <copyright file="ArrivalSystem.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using System;
using Eventium.Core;
using Eventium.Core.Events;
using Eventium.Core.Systems;
using Eventium.Core.World;

namespace Eventium.Scenarios.SimpleContinuous;

public sealed class ArrivalSystem : ISystem
{
    private double _lastArrivalTime = 0.0;

    public IEnumerable<string> HandledEventTypes => [ContinuousEventTypes.CustomerArrival];

    public void HandleEvent(ISimulationContext context, Event evt)
    {
        // Validate payload type (optional, for type safety)
        _ = evt.GetPayload<CustomerArrivalPayload>();

        var arrivalsCounter = context.Metrics.GetCounter("customer.arrivals");
        var interArrivalHistogram = context.Metrics.GetHistogram("customer.inter_arrival_time");
        var activeCustomersGauge = context.Metrics.GetGauge("customer.active_count");

        var id = context.World.Entities.Count + 1;

        var entity = new Entity(id, ContinuousEntityTypes.Customer);
        entity.AddComponent(ContinuousComponentNames.Arrival, new ArrivalComponent
        {
            ArrivedAt = context.Time
        });

        context.World.AddEntity(entity);

        arrivalsCounter.Increment();
        activeCustomersGauge.Increment();

        if (_lastArrivalTime > 0)
        {
            var interArrivalTime = context.Time - _lastArrivalTime;
            interArrivalHistogram.Observe(interArrivalTime);
        }
        _lastArrivalTime = context.Time;

        Console.WriteLine($"t={context.Time:0.0}s: Customer {id} arrived");

        // schedule next arrival ~10s later (exponential)
        var mean = 10.0;
        var u = context.Rng.NextDouble();
        var dt = -mean * Math.Log(1.0 - u);

        context.ScheduleIn(
            dt: dt,
            type: ContinuousEventTypes.CustomerArrival,
            payload: CustomerArrivalPayload.Instance);
    }
}
