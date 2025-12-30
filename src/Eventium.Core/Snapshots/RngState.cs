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
    /// <param name="state">Optional engine-specific RNG state object for restoration.</param>
    /// <param name="sourceType">Optional source type identifier for validation.</param>
    public RngState(double captureTime, int eventCount, object? state = null, string? sourceType = null)
    {
        CaptureTime = captureTime;
        EventCount = eventCount;
        State = state;
        SourceType = sourceType;
    }

    /// <summary>
    /// Gets the simulation time when this RNG state was captured.
    /// </summary>
    public double CaptureTime { get; }

    /// <summary>
    /// Gets the number of events processed when this RNG state was captured.
    /// </summary>
    public int EventCount { get; }

    /// <summary>
    /// Gets the source type name used to capture the state.
    /// </summary>
    public string? SourceType { get; }

    /// <summary>
    /// Gets the captured engine-specific RNG state.
    /// </summary>
    public object? State { get; }
}
