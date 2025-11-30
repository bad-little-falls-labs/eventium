using System;

namespace Eventium.Core.Time;

/// <summary>
/// Encapsulates time semantics for a simulation.
/// </summary>
public sealed class TimeModel
{
    public TimeMode Mode { get; }
    public double Step { get; }

    public TimeModel(TimeMode mode, double step = 1.0)
    {
        Mode = mode;
        Step = step;
    }

    /// <summary>
    /// Initial simulation time.
    /// </summary>
    public static double InitialTime => 0.0;

    /// <summary>
    /// Returns the next step time for discrete simulations.
    /// </summary>
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
