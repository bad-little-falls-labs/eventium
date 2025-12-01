// <copyright file="SimulationResult.cs" company="bad-little-falls-labs">
//  Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core;

/// <summary>
/// Represents the reason a simulation run terminated.
/// </summary>
public enum SimulationStopReason
{
    /// <summary>
    /// The event queue was exhausted.
    /// </summary>
    QueueEmpty,

    /// <summary>
    /// The simulation reached the specified end time.
    /// </summary>
    TimeReached,

    /// <summary>
    /// The maximum number of events was processed.
    /// </summary>
    MaxEventsReached,

    /// <summary>
    /// The simulation was stopped via Stop() call.
    /// </summary>
    StoppedByUser
}

/// <summary>
/// Contains the results and statistics from a simulation run.
/// </summary>
public sealed class SimulationResult
{

    public SimulationResult(
        SimulationStopReason stopReason,
        double finalTime,
        int eventsProcessed,
        int eventsRemaining,
        TimeSpan wallClockDuration,
        int entityCount)
    {
        StopReason = stopReason;
        FinalTime = finalTime;
        EventsProcessed = eventsProcessed;
        EventsRemaining = eventsRemaining;
        WallClockDuration = wallClockDuration;
        EntityCount = entityCount;
    }

    /// <summary>
    /// Gets the number of entities in the world at the end of the run.
    /// </summary>
    public int EntityCount { get; }

    /// <summary>
    /// Gets the average events processed per second (wall-clock).
    /// </summary>
    public double EventsPerSecond =>
        WallClockDuration.TotalSeconds > 0
            ? EventsProcessed / WallClockDuration.TotalSeconds
            : 0;

    /// <summary>
    /// Gets the total number of events processed during the run.
    /// </summary>
    public int EventsProcessed { get; }

    /// <summary>
    /// Gets the number of events remaining in the queue when the run ended.
    /// </summary>
    public int EventsRemaining { get; }

    /// <summary>
    /// Gets the final simulation time when the run ended.
    /// </summary>
    public double FinalTime { get; }
    /// <summary>
    /// Gets the reason the simulation stopped.
    /// </summary>
    public SimulationStopReason StopReason { get; }

    /// <summary>
    /// Gets the wall-clock duration of the simulation run.
    /// </summary>
    public TimeSpan WallClockDuration { get; }

    public override string ToString()
    {
        return $"SimulationResult {{ " +
               $"StopReason = {StopReason}, " +
               $"FinalTime = {FinalTime:F2}, " +
               $"EventsProcessed = {EventsProcessed}, " +
               $"EventsRemaining = {EventsRemaining}, " +
               $"WallClock = {WallClockDuration.TotalMilliseconds:F1}ms, " +
               $"EventsPerSec = {EventsPerSecond:F0}, " +
               $"Entities = {EntityCount} }}";
    }
}
