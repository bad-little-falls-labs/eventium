// <copyright file="GaugeTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.Instrumentation;
using Xunit;

namespace Eventium.Core.Tests.Instrumentation;

public sealed class GaugeTests
{

    [Fact]
    public void Constructor_DefaultsToZero()
    {
        var gauge = new Gauge("test");
        Assert.Equal(0.0, gauge.Value);
    }
    [Fact]
    public void Constructor_SetsNameAndInitialValue()
    {
        var gauge = new Gauge("test.metric", 42.0);
        Assert.Equal("test.metric", gauge.Name);
        Assert.Equal(42.0, gauge.Value);
    }

    [Fact]
    public void Decrement_DecreasesValue()
    {
        var gauge = new Gauge("test", 10.0);
        gauge.Decrement();
        Assert.Equal(9.0, gauge.Value);

        gauge.Decrement(3.0);
        Assert.Equal(6.0, gauge.Value);
    }

    [Fact]
    public void Gauge_CanGoNegative()
    {
        var gauge = new Gauge("test", 5.0);
        gauge.Decrement(10.0);
        Assert.Equal(-5.0, gauge.Value);
    }

    [Fact]
    public void Increment_IncreasesValue()
    {
        var gauge = new Gauge("test", 10.0);
        gauge.Increment();
        Assert.Equal(11.0, gauge.Value);

        gauge.Increment(5.0);
        Assert.Equal(16.0, gauge.Value);
    }

    [Fact]
    public void Set_UpdatesValue()
    {
        var gauge = new Gauge("test");
        gauge.Set(100.5);
        Assert.Equal(100.5, gauge.Value);
    }

    [Fact]
    public void Value_CanBeSetDirectly()
    {
        var gauge = new Gauge("test");
        gauge.Value = 99.9;
        Assert.Equal(99.9, gauge.Value);
    }
}
