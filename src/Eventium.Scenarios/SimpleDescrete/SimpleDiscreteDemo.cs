using System.Collections.Generic;
using Eventium.Core;
using Eventium.Core.Time;
using Eventium.Core.World;

namespace Eventium.Scenarios.SimpleDiscrete;

public static class SimpleDiscreteDemo
{
    public static void Run()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Discrete, step: 1.0));

        // World setup
        var agent = new Entity(id: 1, type: "AGENT");
        agent.AddComponent("position", new PositionComponent { X = 0, Y = 0 });
        engine.World.AddEntity(agent);

        // Systems
        engine.RegisterSystem(new MovementSystem());

        // First move at turn 1
        engine.Schedule(
            time: 1.0,
            type: "MOVE_AGENT",
            payload: new Dictionary<string, object?>
            {
                ["entityId"] = 1,
                ["dx"] = 1,
                ["dy"] = 0
            });

        engine.Run(untilTime: 5.0);
    }
}
