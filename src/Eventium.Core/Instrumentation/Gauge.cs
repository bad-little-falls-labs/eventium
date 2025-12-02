// <copyright file="Gauge.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.Instrumentation;

/// <summary>
/// Tracks a single numeric value that can go up or down.
/// </summary>
public sealed class Gauge
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Gauge"/> class.
    /// </summary>
    /// <param name="name">The name of the gauge.</param>
    /// <param name="initialValue">The initial value (default is 0).</param>
    public Gauge(string name, double initialValue = 0.0)
    {
        Name = name;
        Value = initialValue;
    }

    /// <summary>
    /// Gets the name of this gauge.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets or sets the current value of the gauge.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Decrements the gauge by the specified amount.
    /// </summary>
    /// <param name="amount">The amount to decrement by (default is 1).</param>
    public void Decrement(double amount = 1.0)
    {
        Value -= amount;
    }

    /// <summary>
    /// Increments the gauge by the specified amount.
    /// </summary>
    /// <param name="amount">The amount to increment by (default is 1).</param>
    public void Increment(double amount = 1.0)
    {
        Value += amount;
    }

    /// <summary>
    /// Sets the gauge to the specified value.
    /// </summary>
    /// <param name="value">The new value.</param>
    public void Set(double value)
    {
        Value = value;
    }
}
