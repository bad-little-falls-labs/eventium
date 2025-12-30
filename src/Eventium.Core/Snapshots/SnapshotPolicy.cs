// <copyright file="SnapshotPolicy.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Core.Snapshots;

/// <summary>
/// Defines when and how frequently snapshots should be captured during simulation.
/// </summary>
public sealed class SnapshotPolicy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SnapshotPolicy"/> class.
    /// </summary>
    /// <param name="eventInterval">Optional: capture a snapshot every N events processed. If null, event-based capturing is disabled.</param>
    /// <param name="timeInterval">Optional: capture a snapshot every Δt simulation time. If null, time-based capturing is disabled.</param>
    /// <param name="maxSnapshots">Maximum number of snapshots to retain in the ring buffer (default 100).</param>
    /// <exception cref="ArgumentException">Thrown if both intervals are null, or if maxSnapshots is less than 1.</exception>
    public SnapshotPolicy(int? eventInterval = null, double? timeInterval = null, int maxSnapshots = 100)
    {
        if (eventInterval is null && timeInterval is null)
        {
            throw new ArgumentException(
                "At least one of eventInterval or timeInterval must be specified.",
                nameof(eventInterval));
        }

        if (maxSnapshots < 1)
        {
            throw new ArgumentException("maxSnapshots must be at least 1.", nameof(maxSnapshots));
        }

        EventInterval = eventInterval;
        TimeInterval = timeInterval;
        MaxSnapshots = maxSnapshots;
    }

    /// <summary>
    /// Gets the event interval for snapshot capturing (every N events), or null if disabled.
    /// </summary>
    public int? EventInterval { get; }

    /// <summary>
    /// Gets the maximum number of snapshots to retain in the ring buffer.
    /// </summary>
    public int MaxSnapshots { get; }

    /// <summary>
    /// Gets the time interval for snapshot capturing (every Δt simulation time), or null if disabled.
    /// </summary>
    public double? TimeInterval { get; }
}
