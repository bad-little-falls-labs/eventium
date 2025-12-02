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
}
