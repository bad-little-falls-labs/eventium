// <copyright file="Counter.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using System.Diagnostics.CodeAnalysis;

namespace Eventium.Core.Instrumentation;

/// <summary>
/// Simple monotonically increasing counter.
/// </summary>
public sealed class Counter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Counter"/> class.
    /// </summary>
    /// <param name="name">The name of the counter.</param>
    public Counter(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the name of this counter.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the current value of the counter.
    /// </summary>
    public long Value { get; private set; }

    /// <summary>
    /// Increments the counter by the specified amount.
    /// </summary>
    /// <param name="amount">The amount to increment by (default is 1).</param>
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Modifies instance state (Value)")]
    public void Increment(long amount = 1)
    {
        Value += amount;
    }
}
