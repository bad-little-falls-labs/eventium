// <copyright file="SimulationStepResult.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Core;

/// <summary>
/// Represents the result of processing a batch of simulation steps.
/// </summary>
public sealed class SimulationStepResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationStepResult"/> class.
    /// </summary>
    /// <param name="stopReason">The reason the step processing stopped.</param>
    /// <param name="finalTime">The final simulation time after processing.</param>
    /// <param name="eventsProcessed">The number of events processed in this batch.</param>
    /// <param name="eventsRemaining">The number of events remaining in the queue.</param>
    /// <param name="wallClockDuration">The wall-clock duration of the step batch.</param>
    public SimulationStepResult(
        SimulationStopReason stopReason,
        double finalTime,
        int eventsProcessed,
        int eventsRemaining,
        TimeSpan wallClockDuration)
    {
        StopReason = stopReason;
        FinalTime = finalTime;
        EventsProcessed = eventsProcessed;
        EventsRemaining = eventsRemaining;
        WallClockDuration = wallClockDuration;
    }

    /// <summary>
    /// Gets a value indicating whether the simulation can continue processing (queue not empty and not stopped by user).
    /// </summary>
    public bool CanContinue => StopReason != SimulationStopReason.StoppedByUser && EventsRemaining > 0;

    /// <summary>
    /// Gets the number of events processed in this batch.
    /// </summary>
    public int EventsProcessed { get; }

    /// <summary>
    /// Gets the number of events remaining in the queue.
    /// </summary>
    public int EventsRemaining { get; }

    /// <summary>
    /// Gets the final simulation time after processing.
    /// </summary>
    public double FinalTime { get; }

    /// <summary>
    /// Gets the reason the step processing stopped.
    /// </summary>
    public SimulationStopReason StopReason { get; }

    /// <summary>
    /// Gets the wall-clock duration of the step batch.
    /// </summary>
    public TimeSpan WallClockDuration { get; }
}
