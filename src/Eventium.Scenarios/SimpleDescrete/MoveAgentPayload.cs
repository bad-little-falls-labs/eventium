using Eventium.Core.Events;

namespace Eventium.Scenarios.SimpleDiscrete;

/// <summary>
/// Payload for the MoveAgent event.
/// </summary>
public sealed record MoveAgentPayload(int EntityId, int Dx, int Dy) : IEventPayload;
