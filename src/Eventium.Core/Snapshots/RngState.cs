// <copyright file="RngState.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Core.Snapshots;

/// <summary>
/// Captures the state of a random number generator at a point in time.
/// Note: System.Random does not expose its internal state, so this captures only metadata
/// and cannot fully restore RNG state. For deterministic reproducibility, use seeded engines consistently.
/// </summary>
public sealed class RngState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RngState"/> class.
    /// </summary>
    /// <param name="captureTime">The simulation time when this RNG state was captured.</param>
    /// <param name="eventCount">The number of events processed when captured.</param>
    public RngState(double captureTime, int eventCount)
    {
        CaptureTime = captureTime;
        EventCount = eventCount;
    }

    /// <summary>
    /// Gets the simulation time when this RNG state was captured.
    /// </summary>
    public double CaptureTime { get; }

    /// <summary>
    /// Gets the number of events processed when this RNG state was captured.
    /// </summary>
    public int EventCount { get; }
}
