using System.Collections.Generic;

namespace Eventium.Core.Events;

/// <summary>
/// Priority queue of simulation events, ordered by time then priority.
/// </summary>
public sealed class EventQueue
{
    private readonly PriorityQueue<Event, (double time, int priority)> _queue = new();

    public int Count => _queue.Count;

    public void Enqueue(Event evt)
    {
        _queue.Enqueue(evt, (evt.Time, evt.Priority));
    }

    public Event? Dequeue()
    {
        return _queue.TryDequeue(out var evt, out _) ? evt : null;
    }

    public double? PeekTime()
    {
        return _queue.TryPeek(out var evt, out _)
            ? evt.Time
            : null;
    }
}
