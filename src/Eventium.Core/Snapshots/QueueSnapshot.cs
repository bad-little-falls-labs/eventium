// <copyright file="QueueSnapshot.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

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
    public QueueSnapshot(int count, double? nextEventTime)
    {
        Count = count;
        NextEventTime = nextEventTime;
    }

    /// <summary>
    /// Gets the number of events in the queue.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Gets the time of the next event, or null if the queue is empty.
    /// </summary>
    public double? NextEventTime { get; }
}
