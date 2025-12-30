// <copyright file="ISimulationSnapshot.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Core.Snapshots;

/// <summary>
/// Represents a snapshot of a simulation's state at a specific point in time.
/// </summary>
public interface ISimulationSnapshot
{

    /// <summary>
    /// Gets the number of events processed when this snapshot was captured.
    /// </summary>
    int EventsProcessed { get; }

    /// <summary>
    /// Gets the snapshot of the event queue state.
    /// </summary>
    QueueSnapshot Queue { get; }

    /// <summary>
    /// Gets the snapshot of the random number generator state.
    /// </summary>
    RngState Rng { get; }
    /// <summary>
    /// Gets the simulation time when this snapshot was captured.
    /// </summary>
    double Time { get; }

    /// <summary>
    /// Gets the snapshot of the world state.
    /// </summary>
    WorldSnapshot World { get; }
}
