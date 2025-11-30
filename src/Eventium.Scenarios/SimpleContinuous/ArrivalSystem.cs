using System;
using System.Collections.Generic;
using Eventium.Core;
using Eventium.Core.Events;
using Eventium.Core.Systems;
using Eventium.Core.World;

namespace Eventium.Scenarios.SimpleContinuous;

public sealed class ArrivalSystem : ISystem
{
    public IEnumerable<string> HandledEventTypes => new[] { "CUSTOMER_ARRIVAL" };

    public void HandleEvent(SimulationEngine engine, Event evt)
    {
        var id = engine.World.Entities.Count + 1;

        var entity = new Entity(id, "CUSTOMER");
        entity.AddComponent("arrival", new ArrivalComponent
        {
            ArrivedAt = engine.Time
        });

        engine.World.AddEntity(entity);

        Console.WriteLine($"t={engine.Time:0.0}s: Customer {id} arrived");

        // schedule next arrival ~10s later (exponential)
        var mean = 10.0;
        var u = engine.Rng.NextDouble();
        var dt = -mean * Math.Log(1.0 - u);

        engine.ScheduleIn(
            dt: dt,
            type: "CUSTOMER_ARRIVAL",
            payload: new Dictionary<string, object?>());
    }
}
