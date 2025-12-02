// <copyright file="MetricsRegistry.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using System.Collections.Generic;

namespace Eventium.Core.Instrumentation;

/// <summary>
/// Basic registry for counters and other metrics.
/// </summary>
public sealed class MetricsRegistry
{
    private readonly Dictionary<string, Counter> _counters = new();

    /// <summary>
    /// Gets or creates a counter with the specified name.
    /// </summary>
    /// <param name="name">The unique name of the counter.</param>
    /// <returns>The counter instance. If it doesn't exist, creates a new one.</returns>
    public Counter Counter(string name)
    {
        if (!_counters.TryGetValue(name, out var counter))
        {
            counter = new Counter(name);
            _counters[name] = counter;
        }

        return counter;
    }
}
