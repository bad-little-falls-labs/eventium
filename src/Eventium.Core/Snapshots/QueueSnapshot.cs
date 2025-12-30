// <copyright file="QueueSnapshot.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using Eventium.Core.Events;

namespace Eventium.Core.Snapshots;

/// <summary>
/// Captures the state of the event queue at a point in time.
/// </summary>
public sealed class QueueSnapshot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueueSnapshot"/> class.
    /// </summary>
    /// <param name="count">The number of events in the queue.</param>
    /// <param name="nextEventTime">The time of the next event, or null if queue is empty.</param>
    /// <param name="events">Optional: the events currently in the queue.</param>
    /// <param name="nextSequence">Optional: the next sequence value to assign.</param>
    public QueueSnapshot(int count, double? nextEventTime, IReadOnlyList<Event>? events = null, long nextSequence = 0)
    {
        Count = count;
        NextEventTime = nextEventTime;
        Events = events ?? Array.Empty<Event>();
        NextSequence = nextSequence;
    }

    /// <summary>
    /// Gets the number of events in the queue.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets a snapshot of the queued events.
    /// </summary>
    public IReadOnlyList<Event> Events { get; }

    /// <summary>
    /// Gets the time of the next event, or null if the queue is empty.
    /// </summary>
    public double? NextEventTime { get; }

    /// <summary>
    /// Gets the next sequence value to assign on enqueue.
    /// </summary>
    public long NextSequence { get; }
}
