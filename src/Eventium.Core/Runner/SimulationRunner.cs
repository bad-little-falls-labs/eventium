// <copyright file="SimulationRunner.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using System.Diagnostics;
using Eventium.Core.Snapshots;
using Eventium.Core.Time;

namespace Eventium.Core.Runner;

/// <summary>
/// Controls simulation execution with pause/resume, stepping, and real-time pacing.
/// The runner only calls engine methods and never mutates the world directly.
/// </summary>
public sealed class SimulationRunner : ISimulationRunner
{
    private readonly SimulationClock _clock;
    private readonly SnapshotBuffer _snapshotBuffer;
    private bool _paused;
    private Stopwatch? _realtimeStopwatch;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationRunner"/> class.
    /// </summary>
    /// <param name="engine">The simulation engine to drive.</param>
    /// <param name="snapshotCapacity">Maximum number of snapshots to retain for seeking (default 10).</param>
    public SimulationRunner(ISimulationEngine engine, int snapshotCapacity = 10)
    {
        Engine = engine ?? throw new ArgumentNullException(nameof(engine));
        _clock = new SimulationClock();
        _snapshotBuffer = new SnapshotBuffer(snapshotCapacity);
        _paused = false;
    }

    /// <inheritdoc />
    public SimulationClock Clock => _clock;

    /// <inheritdoc />
    public ISimulationEngine Engine { get; }

    /// <inheritdoc />
    public bool IsPaused => _paused;

    /// <inheritdoc />
    public void Pause()
    {
        _clock.Pause();
        _paused = true;
    }

    /// <inheritdoc />
    public void Resume()
    {
        _clock.Resume();
        _paused = false;
    }

    /// <inheritdoc />
    public async Task RunRealTimeAsync(int frameDurationMs, int eventBudgetPerFrame = 1000, CancellationToken cancellationToken = default)
    {
        if (frameDurationMs <= 0)
        {
            throw new ArgumentException("Frame duration must be positive.", nameof(frameDurationMs));
        }

        if (eventBudgetPerFrame <= 0)
        {
            throw new ArgumentException("Event budget must be positive.", nameof(eventBudgetPerFrame));
        }

        _realtimeStopwatch = Stopwatch.StartNew();
        var lastWallTimeMs = 0.0;

        try
        {
            while (!cancellationToken.IsCancellationRequested && Engine.Queue.Count > 0)
            {
                var frameStart = Stopwatch.GetTimestamp();

                if (!_paused)
                {
                    // Compute target simulation time based on incremental wall elapsed and current time scale
                    var currentWallTimeMs = _realtimeStopwatch.Elapsed.TotalMilliseconds;
                    var wallDeltaMs = currentWallTimeMs - lastWallTimeMs;
                    var simDeltaMs = wallDeltaMs * Clock.TimeScale;
                    var targetSimTime = Engine.Time + (simDeltaMs / 1000.0);
                    lastWallTimeMs = currentWallTimeMs;

                    // Process events up to target time with budget
                    var result = Engine.ProcessUntil(targetSimTime, eventBudgetPerFrame);

                    // Update snapshot buffer periodically (every 10 events processed)
                    if (result.EventsProcessed > 0 && Engine.EventsProcessed % 10 == 0)
                    {
                        if (Engine is ISimulationEngine simulationEngine)
                        {
                            var snapshot = simulationEngine.CaptureSnapshot();
                            _snapshotBuffer.Add(snapshot);
                        }
                    }
                }
                else
                {
                    // Keep lastWallTimeMs updated while paused to prevent time jumps on resume
                    lastWallTimeMs = _realtimeStopwatch.Elapsed.TotalMilliseconds;
                }

                // Frame pacing: sleep until next frame
                var frameElapsedMs = Stopwatch.GetElapsedTime(frameStart).TotalMilliseconds;
                var sleepMs = (int)(frameDurationMs - frameElapsedMs);
                if (sleepMs > 0)
                {
                    await Task.Delay(sleepMs, cancellationToken);
                }
            }
        }
        finally
        {
            _realtimeStopwatch?.Stop();
        }
    }

    /// <inheritdoc />
    public SimulationStepResult Seek(double targetTime)
    {
        if (targetTime < 0)
        {
            throw new ArgumentException("Target time must be non-negative.", nameof(targetTime));
        }

        // If target is before current time, restore from latest snapshot at or before target, then replay forward
        if (targetTime < Engine.Time)
        {
            if (_snapshotBuffer.TryGetLatestAtOrBefore(targetTime, out var snapshot))
            {
                Engine.RestoreSnapshot(snapshot!);

                // If snapshot time matches target exactly, we're done
                if (Math.Abs(snapshot!.Time - targetTime) < 1e-9)
                {
                    return new SimulationStepResult(
                        stopReason: SimulationStopReason.TimeReached,
                        finalTime: Engine.Time,
                        eventsProcessed: 0,
                        eventsRemaining: Engine.Queue.Count,
                        wallClockDuration: TimeSpan.Zero);
                }

                // Otherwise, replay forward from snapshot to target time
                return Engine.ProcessUntil(targetTime);
            }

            // No snapshot found; cannot rewind
            throw new InvalidOperationException(
                $"Cannot seek to {targetTime}: no snapshot available at or before that time. Current time is {Engine.Time}.");
        }

        // If target is current time, return without processing
        if (Math.Abs(targetTime - Engine.Time) < 1e-9)
        {
            return new SimulationStepResult(
                stopReason: SimulationStopReason.TimeReached,
                finalTime: Engine.Time,
                eventsProcessed: 0,
                eventsRemaining: Engine.Queue.Count,
                wallClockDuration: TimeSpan.Zero);
        }

        // Target is ahead; process until reaching it
        return Engine.ProcessUntil(targetTime);
    }

    /// <inheritdoc />
    public void SetTimeScale(double scale)
    {
        if (scale <= 0)
        {
            throw new ArgumentException("Time scale must be greater than 0.", nameof(scale));
        }

        Clock.TimeScale = scale;
    }

    /// <inheritdoc />
    public SimulationStepResult StepDelta(double dt)
    {
        if (dt < 0)
        {
            throw new ArgumentException("Time delta must be non-negative.", nameof(dt));
        }

        var targetTime = Engine.Time + dt;
        return Engine.ProcessUntil(targetTime);
    }

    /// <inheritdoc />
    public SimulationStepResult StepEvent()
    {
        if (Engine.TryProcessNextEvent(out _))
        {
            return new SimulationStepResult(
                stopReason: SimulationStopReason.MaxEventsReached,
                finalTime: Engine.Time,
                eventsProcessed: 1,
                eventsRemaining: Engine.Queue.Count,
                wallClockDuration: TimeSpan.Zero);
        }

        return new SimulationStepResult(
            stopReason: SimulationStopReason.QueueEmpty,
            finalTime: Engine.Time,
            eventsProcessed: 0,
            eventsRemaining: Engine.Queue.Count,
            wallClockDuration: TimeSpan.Zero);
    }

    /// <inheritdoc />
    public SimulationStepResult StepTurn()
    {
        if (Engine.TimeModel.Mode != TimeMode.Discrete)
        {
            throw new InvalidOperationException("StepTurn is only valid for discrete simulations.");
        }

        var nextBoundary = Engine.Time + Engine.TimeModel.Step;
        return Engine.ProcessUntil(nextBoundary);
    }

    /// <inheritdoc />
    public void Stop()
    {
        Engine.Stop();
    }

    /// <summary>
    /// Adds a snapshot to the buffer for testing. Internal for test injection only.
    /// </summary>
    /// <param name="snapshot">The snapshot to add.</param>
    internal void AddSnapshotForTesting(ISimulationSnapshot snapshot)
    {
        _snapshotBuffer.Add(snapshot);
    }
}
