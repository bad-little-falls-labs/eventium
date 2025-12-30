// <copyright file="SimulationSnapshot.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Core.Snapshots;

/// <summary>
/// Concrete implementation of ISimulationSnapshot.
/// </summary>
public sealed class SimulationSnapshot : ISimulationSnapshot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationSnapshot"/> class.
    /// </summary>
    /// <param name="time">The simulation time when the snapshot was captured.</param>
    /// <param name="eventsProcessed">The number of events processed when the snapshot was captured.</param>
    /// <param name="world">The world state snapshot.</param>
    /// <param name="queue">The event queue state snapshot.</param>
    /// <param name="rng">The random number generator state snapshot.</param>
    public SimulationSnapshot(
        double time,
        int eventsProcessed,
        WorldSnapshot world,
        QueueSnapshot queue,
        RngState rng)
    {
        Time = time;
        EventsProcessed = eventsProcessed;
        World = world;
        Queue = queue;
        Rng = rng;
    }

    /// <inheritdoc />
    public int EventsProcessed { get; }

    /// <inheritdoc />
    public QueueSnapshot Queue { get; }

    /// <inheritdoc />
    public RngState Rng { get; }

    /// <inheritdoc />
    public double Time { get; }

    /// <inheritdoc />
    public WorldSnapshot World { get; }
}
