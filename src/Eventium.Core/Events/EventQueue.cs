// <copyright file="EventQueue.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Eventium.Core.Snapshots;

namespace Eventium.Core.Events;

/// <summary>
/// Priority queue of simulation events, ordered by time then priority.
/// </summary>
public sealed class EventQueue : IStatefulEventQueue
{
    private readonly PriorityQueue<Event, (double time, int priority, long sequence)> _queue = new();
    private long _sequenceCounter;

    /// <summary>
    /// Gets the number of events currently in the queue.
    /// </summary>
    public int Count => _queue.Count;

    /// <inheritdoc />
    public long NextSequence => _sequenceCounter;

    /// <inheritdoc />
    public QueueSnapshot CaptureSnapshot()
    {
        var events = _queue.UnorderedItems
            .Select(item => CloneEvent(item.Element))
            .ToList();

        return new QueueSnapshot(
            count: _queue.Count,
            nextEventTime: PeekTime(),
            events: events,
            nextSequence: _sequenceCounter);
    }

    /// <summary>
    /// Removes and returns the next event, or null if the queue is empty.
    /// </summary>
    /// <returns>The next event in priority order, or null if the queue is empty.</returns>
    public Event? Dequeue()
    {
        return _queue.TryDequeue(out var evt, out _) ? evt : null;
    }

    /// <summary>
    /// Adds an event to the queue.
    /// </summary>
    /// <param name="evt">The event to add to the queue.</param>
    public void Enqueue(Event evt)
    {
        evt.Sequence = Interlocked.Increment(ref _sequenceCounter);
        _queue.Enqueue(evt, (evt.Time, evt.Priority, evt.Sequence));
    }

    /// <summary>
    /// Returns the time of the next event without removing it, or null if empty.
    /// </summary>
    /// <returns>The time of the next event, or null if the queue is empty.</returns>
    public double? PeekTime()
    {
        return _queue.TryPeek(out var evt, out _)
            ? evt.Time
            : null;
    }

    /// <inheritdoc />
    public void RestoreSnapshot(QueueSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        _queue.Clear();
        _sequenceCounter = snapshot.NextSequence;

        foreach (var evt in snapshot.Events)
        {
            _queue.Enqueue(evt, (evt.Time, evt.Priority, evt.Sequence));
        }
    }

    private static Event CloneEvent(Event evt)
    {
        ArgumentNullException.ThrowIfNull(evt);

        Event clone;
        if (evt.TypedPayload is null)
        {
            clone = new Event(evt.Time, evt.Priority, evt.Type, new Dictionary<string, object?>(evt.Payload), evt.Handler);
        }
        else
        {
            clone = new Event(evt.Time, evt.Priority, evt.Type, evt.TypedPayload, evt.Handler);
        }

        clone.Sequence = evt.Sequence;
        return clone;
    }
}
