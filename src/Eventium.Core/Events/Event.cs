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

    public override bool Equals(object? obj)
    {
        if (obj is Event other)
        {
            return Math.Abs(Time - other.Time) < double.Epsilon && Priority == other.Priority && Type == other.Type;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Time, Priority, Type);
    }

    public static bool operator ==(Event? left, Event? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Event? left, Event? right)
    {
        return !(left == right);
    }

    public static bool operator <(Event? left, Event? right)
    {
        if (left is null) return right is not null;
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Event? left, Event? right)
    {
        if (left is null) return true;
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Event? left, Event? right)
    {
        if (left is null) return false;
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Event? left, Event? right)
    {
        if (left is null) return right is null;
        return left.CompareTo(right) >= 0;
    }
}
