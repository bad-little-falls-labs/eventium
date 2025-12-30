// <copyright file="SimulationClock.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Core.Time;

/// <summary>
/// Controls the execution pacing and stepping behavior of a simulation.
/// The core engine remains fully deterministic; this clock is used by runners to control how stepping occurs.
/// </summary>
public sealed class SimulationClock
{
    private double _timeScale = 1.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationClock"/> class.
    /// </summary>
    /// <param name="mode">The time mode (Discrete or Continuous).</param>
    /// <param name="stepPolicy">The stepping policy (Event, Tick, or Turn).</param>
    public SimulationClock(TimeMode mode, StepPolicy stepPolicy = StepPolicy.Event)
    {
        Mode = mode;
        StepPolicy = stepPolicy;
    }

    /// <summary>
    /// Gets the time mode (Discrete or Continuous) for this simulation.
    /// </summary>
    public TimeMode Mode { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the simulation is paused.
    /// </summary>
    public bool IsPaused { get; set; }

    /// <summary>
    /// Gets or sets the time scale multiplier for simulation speed.
    /// Values greater than 1.0 speed up the simulation; values between 0 and 1.0 slow it down.
    /// Default is 1.0 (normal speed).
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if value is less than or equal to 0.</exception>
    public double TimeScale
    {
        get => _timeScale;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("TimeScale must be greater than 0.", nameof(value));
            }

            _timeScale = value;
        }
    }

    /// <summary>
    /// Gets or sets the stepping policy (how the simulation advances).
    /// </summary>
    public StepPolicy StepPolicy { get; set; }

    /// <summary>
    /// Returns a string representation of this clock's configuration.
    /// </summary>
    public override string ToString()
    {
        var paused = IsPaused ? "PAUSED" : "RUNNING";
        return $"SimulationClock[Mode={Mode}, Policy={StepPolicy}, TimeScale={TimeScale:F2}x, State={paused}]";
    }
}
