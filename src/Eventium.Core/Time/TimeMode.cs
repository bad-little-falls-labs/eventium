namespace Eventium.Core.Time;

/// <summary>
/// Indicates how simulation time is modeled.
/// </summary>
public enum TimeMode
{
    /// <summary>Discrete steps / turns (0, 1, 2, ...).</summary>
    Discrete,

    /// <summary>Continuous time (e.g. seconds, minutes).</summary>
    Continuous
}
