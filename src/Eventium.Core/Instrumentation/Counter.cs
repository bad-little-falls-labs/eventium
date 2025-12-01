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

    public Counter(string name)
    {
        Name = name;
    }
    public string Name { get; }
    public long Value { get; private set; }

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Modifies instance state (Value)")]
    public void Increment(long amount = 1)
    {
        Value += amount;
    }
}
