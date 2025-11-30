using System;
using System.Collections.Generic;
using Eventium.Core;
using Eventium.Core.Events;
using Eventium.Core.Systems;

namespace Eventium.Scenarios.SimpleDiscrete;

public sealed class MovementSystem : ISystem
{
    public IEnumerable<string> HandledEventTypes => [DiscreteEventTypes.MoveAgent];

    public void HandleEvent(ISimulationContext context, Event evt)
    {
        var payload = evt.GetPayload<MoveAgentPayload>();

        var entity = context.World.GetEntity(payload.EntityId)!;
        var pos = entity.GetComponent<PositionComponent>(DiscreteComponentNames.Position)!;

        pos.X += payload.Dx;
        pos.Y += payload.Dy;

        var turn = context.TimeModel.ToTurn(context.Time);
        Console.WriteLine($"Turn {turn}: Agent {payload.EntityId} moved to ({pos.X},{pos.Y})");

        // Schedule another move next turn
        context.ScheduleIn(
            dt: 1.0,
            type: DiscreteEventTypes.MoveAgent,
            payload: new MoveAgentPayload(payload.EntityId, payload.Dx, payload.Dy));
    }
}
