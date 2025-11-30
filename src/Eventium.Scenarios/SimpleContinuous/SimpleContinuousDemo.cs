using System;
using Eventium.Core;
using Eventium.Core.Time;

namespace Eventium.Scenarios.SimpleContinuous;

public static class SimpleContinuousDemo
{
    public static SimulationResult Run()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous), seed: 42);

        engine.RegisterSystem(new ArrivalSystem());

        // Initial arrival at t=0
        engine.Schedule(
            time: 0.0,
            type: ContinuousEventTypes.CustomerArrival,
            payload: CustomerArrivalPayload.Instance);

        var result = engine.Run(untilTime: 60.0);
        Console.WriteLine(result);
        return result;
    }
}
