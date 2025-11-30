namespace Eventium.Core.Events;

/// <summary>
/// Abstraction for a priority queue of simulation events.
/// </summary>
public interface IEventQueue
{
    /// <summary>
    /// Gets the number of events currently in the queue.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Removes and returns the next event, or null if the queue is empty.
    /// </summary>
    Event? Dequeue();

    /// <summary>
    /// Adds an event to the queue.
    /// </summary>
    void Enqueue(Event evt);

    /// <summary>
    /// Returns the time of the next event without removing it, or null if empty.
    /// </summary>
    double? PeekTime();
}
