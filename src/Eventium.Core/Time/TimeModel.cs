// <copyright file="TimeModel.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using System;

namespace Eventium.Core.Time;

/// <summary>
/// Encapsulates time semantics for a simulation.
/// </summary>
public sealed class TimeModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimeModel"/> class.
    /// </summary>
    /// <param name="mode">The time mode (Discrete or Continuous).</param>
    /// <param name="step">The time step increment for discrete simulations (default is 1.0).</param>
    public TimeModel(TimeMode mode, double step = 1.0)
    {
        Mode = mode;
        Step = step;
    }

    /// <summary>
    /// Gets the initial simulation time (always 0.0).
    /// </summary>
    public static double InitialTime => 0.0;

    /// <summary>
    /// Gets the time mode for this simulation.
    /// </summary>
    public TimeMode Mode { get; }

    /// <summary>
    /// Gets the time step increment for discrete simulations.
    /// </summary>
    public double Step { get; }

    /// <summary>
    /// Returns the next step time for discrete simulations.
    /// </summary>
    /// <param name="current">The current simulation time.</param>
    /// <returns>The time of the next discrete step.</returns>
    /// <exception cref="InvalidOperationException">Thrown if mode is not Discrete.</exception>
    public double NextStepTime(double current)
    {
        if (Mode != TimeMode.Discrete)
        {
            throw new InvalidOperationException(
                "NextStepTime is only valid for discrete simulations.");
        }

        return current + Step;
    }

    /// <summary>
    /// Converts a time value to an integer turn index for discrete simulations.
    /// </summary>
    /// <param name="time">The simulation time to convert.</param>
    /// <returns>The turn number (integer index).</returns>
    /// <exception cref="InvalidOperationException">Thrown if mode is not Discrete.</exception>
    public int ToTurn(double time)
    {
        if (Mode != TimeMode.Discrete)
        {
            throw new InvalidOperationException(
                "ToTurn is only valid for discrete simulations.");
        }

        return (int)Math.Round(time / Step);
    }
}
