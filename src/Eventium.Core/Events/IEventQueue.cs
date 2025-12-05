// <copyright file="IEventQueue.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
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
    /// <returns>The next event in priority order, or null if the queue is empty.</returns>
    Event? Dequeue();

    /// <summary>
    /// Adds an event to the queue.
    /// </summary>
    /// <param name="evt">The event to add to the queue.</param>
    void Enqueue(Event evt);

    /// <summary>
    /// Returns the time of the next event without removing it, or null if empty.
    /// </summary>
    /// <returns>The time of the next event, or null if the queue is empty.</returns>
    double? PeekTime();
}
