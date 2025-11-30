using System;
using System.Collections.Generic;
using Eventium.Core;
using Eventium.Core.Events;
using Eventium.Core.Systems;

namespace Eventium.Scenarios.SimpleDiscrete;

public sealed class MovementSystem : ISystem
{
    public IEnumerable<string> HandledEventTypes => new[] { "MOVE_AGENT" };

    public void HandleEvent(SimulationEngine engine, Event evt)
    {
        var entityId = (int)evt.Payload["entityId"]!;
        var dx = (int)evt.Payload["dx"]!;
        var dy = (int)evt.Payload["dy"]!;

        var entity = engine.World.GetEntity(entityId)!;
        var pos = entity.GetComponent<PositionComponent>("position")!;

        pos.X += dx;
        pos.Y += dy;

        var turn = engine.TimeModel.ToTurn(engine.Time);
        Console.WriteLine($"Turn {turn}: Agent {entityId} moved to ({pos.X},{pos.Y})");

        // Schedule another move next turn
        engine.ScheduleIn(
            dt: 1.0,
            type: "MOVE_AGENT",
            payload: new Dictionary<string, object?>
            {
                ["entityId"] = entityId,
                ["dx"] = dx,
                ["dy"] = dy
            });
    }
}
