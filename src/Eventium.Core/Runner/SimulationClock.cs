// <copyright file="SimulationClock.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.Runner;

/// <summary>
/// Controls pacing and time scaling for simulation execution.
/// </summary>
public sealed class SimulationClock
{
    private bool _isPaused;
    private double _timeScale = 1.0;

    /// <summary>
    /// Gets a value indicating whether the clock is paused.
    /// </summary>
    public bool IsPaused => _isPaused;

    /// <summary>
    /// Gets or sets the time scale multiplier for real-time pacing.
    /// 1.0 = normal speed, 2.0 = 2x speed, 0.5 = half speed.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if scale is not greater than 0.</exception>
    public double TimeScale
    {
        get => _timeScale;
        set
        {
            if (value <= 0)
                throw new ArgumentException("TimeScale must be greater than 0.", nameof(value));
            _timeScale = value;
        }
    }

    /// <summary>
    /// Pauses the clock.
    /// </summary>
    public void Pause()
    {
        _isPaused = true;
    }

    /// <summary>
    /// Resumes the clock from a paused state.
    /// </summary>
    public void Resume()
    {
        _isPaused = false;
    }
}
