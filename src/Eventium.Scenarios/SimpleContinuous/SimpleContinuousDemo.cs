using System.Collections.Generic;
using Eventium.Core;
using Eventium.Core.Events;
using Eventium.Core.Time;

namespace Eventium.Scenarios.SimpleContinuous;

public static class SimpleContinuousDemo
{
    public static void Run()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous), seed: 42);

        engine.RegisterSystem(new ArrivalSystem());

        // Initial arrival at t=0
        engine.Schedule(
            time: 0.0,
            type: "CUSTOMER_ARRIVAL",
            payload: new Dictionary<string, object?>());

        engine.Run(untilTime: 60.0);
    }
}
