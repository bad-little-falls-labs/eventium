// <copyright file="Histogram.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.Instrumentation;

/// <summary>
/// Tracks distribution of observed values with summary statistics.
/// </summary>
public sealed class Histogram
{
    private readonly List<double> _observations = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Histogram"/> class.
    /// </summary>
    /// <param name="name">The name of the histogram.</param>
    public Histogram(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the number of observations recorded.
    /// </summary>
    public int Count => _observations.Count;

    /// <summary>
    /// Gets the maximum observed value, or null if no observations.
    /// </summary>
    public double? Max => _observations.Count > 0 ? _observations.Max() : null;

    /// <summary>
    /// Gets the mean (average) of observed values, or null if no observations.
    /// </summary>
    public double? Mean => _observations.Count > 0 ? _observations.Average() : null;

    /// <summary>
    /// Gets the median value (50th percentile).
    /// </summary>
    public double? Median => Percentile(50);

    /// <summary>
    /// Gets the minimum observed value, or null if no observations.
    /// </summary>
    public double? Min => _observations.Count > 0 ? _observations.Min() : null;

    /// <summary>
    /// Gets the name of this histogram.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the sum of all observed values.
    /// </summary>
    public double Sum => _observations.Sum();

    /// <summary>
    /// Records a new observation.
    /// </summary>
    /// <param name="value">The value to observe.</param>
    public void Observe(double value)
    {
        _observations.Add(value);
    }

    /// <summary>
    /// Calculates the specified percentile value.
    /// </summary>
    /// <param name="percentile">The percentile to calculate (0-100).</param>
    /// <returns>The value at the specified percentile, or null if no observations.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if percentile is not between 0 and 100.</exception>
    public double? Percentile(double percentile)
    {
        if (percentile < 0 || percentile > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(percentile), "Percentile must be between 0 and 100");
        }

        if (_observations.Count == 0)
        {
            return null;
        }

        var sorted = _observations.OrderBy(x => x).ToList();
        var index = (percentile / 100.0) * (sorted.Count - 1);
        var lower = (int)Math.Floor(index);
        var upper = (int)Math.Ceiling(index);

        if (lower == upper)
        {
            return sorted[lower];
        }

        var fraction = index - lower;
        return sorted[lower] + (fraction * (sorted[upper] - sorted[lower]));
    }

    /// <summary>
    /// Resets all observations.
    /// </summary>
    public void Reset()
    {
        _observations.Clear();
    }
}
