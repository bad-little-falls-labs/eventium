namespace Eventium.Core.Events;

/// <summary>
/// A payload type for events that carry no data.
/// </summary>
public sealed record EmptyPayload : IEventPayload
{
    public static readonly EmptyPayload Instance = new();

    private EmptyPayload() { }
}
