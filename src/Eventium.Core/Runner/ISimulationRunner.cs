// <copyright file="ISimulationRunner.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.Runner;

/// <summary>
/// Abstraction for driving a simulation engine with pause/resume, stepping, and seeking capabilities.
/// The runner never mutates the world directly—it only calls engine methods.
/// </summary>
public interface ISimulationRunner
{
    /// <summary>
    /// Gets the underlying simulation engine being driven.
    /// </summary>
    ISimulationEngine Engine { get; }

    /// <summary>
    /// Gets the simulation clock controlling pacing and stepping behavior.
    /// </summary>
    SimulationClock Clock { get; }

    /// <summary>
    /// Gets a value indicating whether the runner is currently paused.
    /// </summary>
    bool IsPaused { get; }

    /// <summary>
    /// Pauses the simulation. Manual stepping is still allowed while paused.
    /// </summary>
    void Pause();

    /// <summary>
    /// Resumes the simulation from a paused state.
    /// </summary>
    void Resume();

    /// <summary>
    /// Stops the simulation permanently.
    /// </summary>
    void Stop();

    /// <summary>
    /// Processes the next event in the queue and returns the result.
    /// </summary>
    /// <returns>The step result.</returns>
    SimulationStepResult StepEvent();

    /// <summary>
    /// Advances one turn (discrete simulations) or processes events up to the next major boundary.
    /// For discrete simulations, this processes events up to and including <c>currentTime + timeModel.Step</c>.
    /// </summary>
    /// <returns>The step result.</returns>
    SimulationStepResult StepTurn();

    /// <summary>
    /// Advances the simulation by a time delta (primarily for continuous simulations).
    /// Processes events up to and including <c>currentTime + dt</c>.
    /// </summary>
    /// <param name="dt">The time delta to advance.</param>
    /// <returns>The step result.</returns>
    SimulationStepResult StepDelta(double dt);

    /// <summary>
    /// Sets the time scale for real-time pacing (1.0 = normal, 2.0 = 2x speed, 0.5 = half speed).
    /// </summary>
    /// <param name="scale">The time scale multiplier. Must be greater than 0.</param>
    void SetTimeScale(double scale);

    /// <summary>
    /// Seeks to a target simulation time via snapshot restore and replay.
    /// If the target is before the current time and a snapshot exists, restores and replays.
    /// If the target is ahead, processes events until reaching it.
    /// </summary>
    /// <param name="targetTime">The simulation time to seek to.</param>
    /// <returns>The final step result from the seek operation.</returns>
    SimulationStepResult Seek(double targetTime);

    /// <summary>
    /// Runs the simulation in real-time, advancing by <c>wallElapsed * timeScale</c> per frame.
    /// Processes events in batches with an optional budget to prevent CPU runaway.
    /// </summary>
    /// <param name="frameDurationMs">Target frame duration in milliseconds (e.g., 16 for ~60 FPS).</param>
    /// <param name="eventBudgetPerFrame">Maximum events to process per frame (default 1000). Prevents runaway when many events occur at the same simulation time.</param>
    /// <param name="cancellationToken">Token to stop the real-time loop.</param>
    /// <returns>A task that completes when the simulation stops or cancellation is requested.</returns>
    Task RunRealTimeAsync(int frameDurationMs, int eventBudgetPerFrame = 1000, CancellationToken cancellationToken = default);
}
