// <copyright file="Event.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.Events;

/// <summary>
/// Represents a scheduled event in the simulation.
/// </summary>
public sealed class Event : IComparable<Event>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Event"/> class with a dictionary payload.
    /// </summary>
    /// <param name="time">The simulation time when this event should occur.</param>
    /// <param name="priority">The priority for ordering events at the same time (lower values execute first).</param>
    /// <param name="type">The event type identifier.</param>
    /// <param name="payload">Optional dictionary payload for event data.</param>
    /// <param name="handler">The handler delegate to invoke when this event executes.</param>
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
        TypedPayload = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Event"/> class with a strongly-typed payload.
    /// </summary>
    /// <param name="time">The simulation time when this event should occur.</param>
    /// <param name="priority">The priority for ordering events at the same time (lower values execute first).</param>
    /// <param name="type">The event type identifier.</param>
    /// <param name="typedPayload">The strongly-typed payload implementing <see cref="IEventPayload"/>.</param>
    /// <param name="handler">The handler delegate to invoke when this event executes.</param>
    public Event(
        double time,
        int priority,
        string type,
        IEventPayload typedPayload,
        EventHandlerDelegate handler)
    {
        Time = time;
        Priority = priority;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Payload = new Dictionary<string, object?>();
        TypedPayload = typedPayload ?? throw new ArgumentNullException(nameof(typedPayload));
        Handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    /// <summary>
    /// Gets the handler delegate for this event.
    /// </summary>
    public EventHandlerDelegate Handler { get; }

    /// <summary>
    /// Gets the dictionary payload (empty if using typed payload).
    /// </summary>
    public IReadOnlyDictionary<string, object?> Payload { get; }

    /// <summary>
    /// Gets the priority for ordering events at the same time.
    /// </summary>
    public int Priority { get; }

    /// <summary>
    /// Gets the simulation time when this event should occur.
    /// </summary>
    public double Time { get; }

    /// <summary>
    /// Gets the event type identifier.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the strongly-typed payload if this event was created with one.
    /// Returns null for events created with dictionary payloads.
    /// </summary>
    public IEventPayload? TypedPayload { get; }

    public static bool operator ==(Event? left, Event? right)
    {
        if (left is null)
            return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Event? left, Event? right)
    {
        return !(left == right);
    }

    public static bool operator <(Event? left, Event? right)
    {
        if (left is null)
            return right is not null;
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Event? left, Event? right)
    {
        if (left is null)
            return true;
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Event? left, Event? right)
    {
        if (left is null)
            return false;
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Event? left, Event? right)
    {
        if (left is null)
            return right is null;
        return left.CompareTo(right) >= 0;
    }

    /// <summary>
    /// Compares this event to another for ordering by time, then priority.
    /// </summary>
    /// <param name="other">The event to compare to.</param>
    /// <returns>Negative if this event comes before other, positive if after, zero if equal.</returns>
    public int CompareTo(Event? other)
    {
        if (other is null)
            return 1;

        var timeCmp = Time.CompareTo(other.Time);
        if (timeCmp != 0)
            return timeCmp;

        return Priority.CompareTo(other.Priority);
    }

    /// <summary>
    /// Determines whether the specified object is equal to this event.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if the objects are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is Event other)
        {
            return Math.Abs(Time - other.Time) < double.Epsilon && Priority == other.Priority && Type == other.Type;
        }
        return false;
    }

    /// <summary>
    /// Gets the hash code for this event.
    /// </summary>
    /// <returns>A hash code computed from time, priority, and type.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Time, Priority, Type);
    }

    /// <summary>
    /// Gets the typed payload cast to the specified type.
    /// </summary>
    /// <typeparam name="TPayload">The expected payload type.</typeparam>
    /// <returns>The typed payload.</returns>
    /// <exception cref="InvalidCastException">If the payload is not of the expected type.</exception>
    public TPayload GetPayload<TPayload>() where TPayload : IEventPayload
    {
        if (TypedPayload is TPayload typed)
            return typed;

        throw new InvalidCastException(
            $"Event payload is not of type {typeof(TPayload).Name}. " +
            $"Actual type: {TypedPayload?.GetType().Name ?? "null (dictionary payload)"}");
    }

    /// <summary>
    /// Tries to get the typed payload cast to the specified type.
    /// </summary>
    /// <typeparam name="TPayload">The expected payload type.</typeparam>
    /// <param name="payload">The typed payload if successful.</param>
    /// <returns>True if the payload was successfully cast.</returns>
    public bool TryGetPayload<TPayload>(out TPayload? payload) where TPayload : class, IEventPayload
    {
        if (TypedPayload is TPayload typed)
        {
            payload = typed;
            return true;
        }

        payload = null;
        return false;
    }
}
