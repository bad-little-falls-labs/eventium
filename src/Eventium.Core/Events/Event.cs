using System;
using System.Collections.Generic;

namespace Eventium.Core.Events;

/// <summary>
/// Represents a scheduled event in the simulation.
/// </summary>
public sealed class Event : IComparable<Event>
{
    public double Time { get; }
    public int Priority { get; }
    public string Type { get; }
    public IReadOnlyDictionary<string, object?> Payload { get; }
    public EventHandlerDelegate Handler { get; }

    public Event(
        double time,
        int priority,
        string type,
        IDictionary<string, object?>? payload,
        EventHandlerDelegate handler)
    {
        Time = time;
        Priority = priority;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Payload = payload is null
            ? new Dictionary<string, object?>()
            : new Dictionary<string, object?>(payload);
        Handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public int CompareTo(Event? other)
    {
        if (other is null) return 1;

        var timeCmp = Time.CompareTo(other.Time);
        if (timeCmp != 0) return timeCmp;

        return Priority.CompareTo(other.Priority);
    }
}
