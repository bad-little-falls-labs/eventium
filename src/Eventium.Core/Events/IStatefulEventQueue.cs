// <copyright file="IStatefulEventQueue.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.Events;

/// <summary>
/// Extends <see cref="IEventQueue"/> with snapshot capture and restoration for deterministic replay.
/// </summary>
public interface IStatefulEventQueue : IEventQueue
{

    /// <summary>
    /// Gets the next sequence value that will be assigned on enqueue.
    /// </summary>
    long NextSequence { get; }
    /// <summary>
    /// Captures the full queue state including scheduled events and the sequence counter.
    /// </summary>
    /// <returns>A snapshot of the queue.</returns>
    Snapshots.QueueSnapshot CaptureSnapshot();

    /// <summary>
    /// Restores the queue state from a prior snapshot.
    /// </summary>
    /// <param name="snapshot">The snapshot to restore.</param>
    void RestoreSnapshot(Snapshots.QueueSnapshot snapshot);
}
