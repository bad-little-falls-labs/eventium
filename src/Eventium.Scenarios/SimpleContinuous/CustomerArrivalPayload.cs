using Eventium.Core.Events;

namespace Eventium.Scenarios.SimpleContinuous;

/// <summary>
/// Payload for the CustomerArrival event.
/// Carries no additional data - uses EmptyPayload pattern.
/// </summary>
public sealed record CustomerArrivalPayload : IEventPayload
{
    public static readonly CustomerArrivalPayload Instance = new();

    private CustomerArrivalPayload() { }
}
