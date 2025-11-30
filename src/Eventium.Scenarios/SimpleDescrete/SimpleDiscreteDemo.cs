using System;
using Eventium.Core;
using Eventium.Core.Time;
using Eventium.Core.World;

namespace Eventium.Scenarios.SimpleDiscrete;

public static class SimpleDiscreteDemo
{
    private const string AgentEntityType = "AGENT";

    public static SimulationResult Run()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Discrete, step: 1.0));

        // World setup
        var agent = new Entity(id: 1, type: AgentEntityType);
        agent.AddComponent(DiscreteComponentNames.Position, new PositionComponent { X = 0, Y = 0 });
        engine.World.AddEntity(agent);

        // Systems
        engine.RegisterSystem(new MovementSystem());

        // First move at turn 1
        engine.Schedule(
            time: 1.0,
            type: DiscreteEventTypes.MoveAgent,
            payload: new MoveAgentPayload(EntityId: 1, Dx: 1, Dy: 0));

        var result = engine.Run(untilTime: 5.0);
        Console.WriteLine(result);
        return result;
    }
}
