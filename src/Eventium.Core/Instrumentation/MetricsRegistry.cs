// <copyright file="MetricsRegistry.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using System.Collections.Concurrent;

namespace Eventium.Core.Instrumentation;

/// <summary>
/// Central registry for counters, histograms, gauges, and other metrics emitted during a simulation run.
/// </summary>
public sealed class MetricsRegistry
{
    private readonly ConcurrentDictionary<string, Counter> _counters = new();
    private readonly ConcurrentDictionary<string, Gauge> _gauges = new();
    private readonly ConcurrentDictionary<string, Histogram> _histograms = new();

    /// <summary>
    /// Returns a snapshot of currently registered counters.
    /// </summary>
    public IReadOnlyDictionary<string, Counter> Counters => _counters;

    /// <summary>
    /// Returns a snapshot of currently registered gauges.
    /// </summary>
    public IReadOnlyDictionary<string, Gauge> Gauges => _gauges;

    /// <summary>
    /// Returns a snapshot of currently registered histograms.
    /// </summary>
    public IReadOnlyDictionary<string, Histogram> Histograms => _histograms;

    /// <summary>
    /// Backwards-compatible alias for <see cref="GetCounter(string)"/>.
    /// </summary>
    /// <param name="name">The unique name of the counter.</param>
    /// <returns>The counter instance.</returns>
    public Counter Counter(string name) => GetCounter(name);

    /// <summary>
    /// Gets or creates a counter with the specified name.
    /// </summary>
    /// <param name="name">The unique name of the counter.</param>
    /// <returns>The counter instance. If it doesn't exist, creates a new one.</returns>
    public Counter GetCounter(string name) => _counters.GetOrAdd(name, n => new Counter(n));

    /// <summary>
    /// Gets or creates a gauge with the specified name.
    /// </summary>
    /// <param name="name">The unique name of the gauge.</param>
    /// <param name="initialValue">The initial value if creating a new gauge (default is 0).</param>
    /// <returns>The gauge instance. If it doesn't exist, creates a new one.</returns>
    public Gauge GetGauge(string name, double initialValue = 0.0)
    {
        return _gauges.GetOrAdd(name, n => new Gauge(n, initialValue));
    }

    /// <summary>
    /// Gets or creates a histogram with the specified name.
    /// </summary>
    /// <param name="name">The unique name of the histogram.</param>
    /// <returns>The histogram instance. If it doesn't exist, creates a new one.</returns>
    public Histogram GetHistogram(string name) => _histograms.GetOrAdd(name, n => new Histogram(n));
}
