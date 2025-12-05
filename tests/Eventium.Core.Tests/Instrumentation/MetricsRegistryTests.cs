// <copyright file="MetricsRegistryTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.Instrumentation;

namespace Eventium.Core.Tests.Instrumentation;

public class MetricsRegistryTests
{

    [Fact]
    public void Counter_ConcurrentAccess_ReturnsSameInstance()
    {
        var registry = new MetricsRegistry();
        var counters = new Counter[10];

        Parallel.For(0, 10, i =>
        {
            counters[i] = registry.Counter("shared_counter");
        });

        var firstCounter = counters[0];
        Assert.All(counters, counter => Assert.Same(firstCounter, counter));
    }

    [Fact]
    public void Counter_ExistingCounter_PreservesValue()
    {
        var registry = new MetricsRegistry();
        var counter1 = registry.Counter("test");
        counter1.Increment(42);

        var counter2 = registry.Counter("test");

        Assert.Equal(42L, counter2.Value);
    }

    [Fact]
    public void Counter_ExistingCounter_ReturnsSameInstance()
    {
        var registry = new MetricsRegistry();
        var counter1 = registry.Counter("test_counter");

        var counter2 = registry.Counter("test_counter");

        Assert.Same(counter1, counter2);
    }

    [Fact]
    public void Counter_MultipleCounters_MaintainsSeparateInstances()
    {
        var registry = new MetricsRegistry();

        var counter1 = registry.Counter("counter1");
        var counter2 = registry.Counter("counter2");

        counter1.Increment(5);
        counter2.Increment(10);

        Assert.Equal(5L, counter1.Value);
        Assert.Equal(10L, counter2.Value);
    }
    [Fact]
    public void Counter_NewCounter_CreatesAndReturnsCounter()
    {
        var registry = new MetricsRegistry();

        var counter = registry.Counter("test_counter");

        Assert.NotNull(counter);
        Assert.Equal("test_counter", counter.Name);
        Assert.Equal(0L, counter.Value);
    }

    [Fact]
    public void GetGauge_ExistingGauge_ReturnsSameInstance()
    {
        var registry = new MetricsRegistry();
        var gauge1 = registry.GetGauge("test");
        gauge1.Set(99.0);
        var gauge2 = registry.GetGauge("test");
        Assert.Same(gauge1, gauge2);
        Assert.Equal(99.0, gauge2.Value);
    }

    [Fact]
    public void GetGauge_NewGauge_CreatesAndReturnsGauge()
    {
        var registry = new MetricsRegistry();
        var gauge = registry.GetGauge("test_gauge", 10.0);
        Assert.NotNull(gauge);
        Assert.Equal("test_gauge", gauge.Name);
        Assert.Equal(10.0, gauge.Value);
    }

    [Fact]
    public void GetHistogram_ExistingHistogram_ReturnsSameInstance()
    {
        var registry = new MetricsRegistry();
        var histogram1 = registry.GetHistogram("test");
        histogram1.Observe(42.0);
        var histogram2 = registry.GetHistogram("test");
        Assert.Same(histogram1, histogram2);
        Assert.Equal(1, histogram2.Count);
    }

    [Fact]
    public void GetHistogram_NewHistogram_CreatesAndReturnsHistogram()
    {
        var registry = new MetricsRegistry();
        var histogram = registry.GetHistogram("test_histogram");
        Assert.NotNull(histogram);
        Assert.Equal("test_histogram", histogram.Name);
        Assert.Equal(0, histogram.Count);
    }

    [Fact]
    public void Snapshots_ReturnReadOnlyCollections()
    {
        var registry = new MetricsRegistry();
        registry.GetCounter("c1");
        registry.GetHistogram("h1");
        registry.GetGauge("g1");
        Assert.Single(registry.Counters);
        Assert.Single(registry.Histograms);
        Assert.Single(registry.Gauges);
    }
}
