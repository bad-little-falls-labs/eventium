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
    public IEnumerable<string> HandledEventTypes => [ContinuousEventTypes.CustomerArrival];

    public void HandleEvent(ISimulationContext context, Event evt)
    {
        // Validate payload type (optional, for type safety)
        _ = evt.GetPayload<CustomerArrivalPayload>();

        var id = context.World.Entities.Count + 1;

        var entity = new Entity(id, ContinuousEntityTypes.Customer);
        entity.AddComponent(ContinuousComponentNames.Arrival, new ArrivalComponent
        {
            ArrivedAt = context.Time
        });

        context.World.AddEntity(entity);

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
