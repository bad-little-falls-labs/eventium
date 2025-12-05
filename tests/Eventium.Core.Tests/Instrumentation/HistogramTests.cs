// <copyright file="HistogramTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.Instrumentation;
using Xunit;

namespace Eventium.Core.Tests.Instrumentation;

public sealed class HistogramTests
{
    [Fact]
    public void Constructor_SetsName()
    {
        var histogram = new Histogram("test.metric");
        Assert.Equal("test.metric", histogram.Name);
    }

    [Fact]
    public void InitialState_IsEmpty()
    {
        var histogram = new Histogram("test");
        Assert.Equal(0, histogram.Count);
        Assert.Null(histogram.Min);
        Assert.Null(histogram.Max);
        Assert.Null(histogram.Mean);
        Assert.Null(histogram.Median);
        Assert.Equal(0, histogram.Sum);
    }

    [Fact]
    public void Median_MatchesP50()
    {
        var histogram = new Histogram("test");
        histogram.Observe(1);
        histogram.Observe(2);
        histogram.Observe(3);
        histogram.Observe(4);
        histogram.Observe(5);

        Assert.Equal(histogram.Percentile(50), histogram.Median);
        Assert.Equal(3, histogram.Median);
    }

    [Fact]
    public void Observe_UpdatesStatistics()
    {
        var histogram = new Histogram("test");
        histogram.Observe(10.0);
        histogram.Observe(20.0);
        histogram.Observe(30.0);

        Assert.Equal(3, histogram.Count);
        Assert.Equal(10.0, histogram.Min);
        Assert.Equal(30.0, histogram.Max);
        Assert.Equal(20.0, histogram.Mean);
        Assert.Equal(60.0, histogram.Sum);
    }

    [Fact]
    public void Percentile_CalculatesCorrectly()
    {
        var histogram = new Histogram("test");
        for (int i = 1; i <= 100; i++)
        {
            histogram.Observe(i);
        }

        Assert.Equal(1.0, histogram.Percentile(0)!.Value);
        Assert.Equal(25.75, histogram.Percentile(25)!.Value, 2);
        Assert.Equal(50.5, histogram.Percentile(50)!.Value, 2);
        Assert.Equal(75.25, histogram.Percentile(75)!.Value, 2);
        Assert.Equal(100.0, histogram.Percentile(100)!.Value);
    }

    [Fact]
    public void Percentile_ReturnsNullWhenEmpty()
    {
        var histogram = new Histogram("test");
        Assert.Null(histogram.Percentile(50));
    }

    [Fact]
    public void Percentile_ThrowsForInvalidPercentile()
    {
        var histogram = new Histogram("test");
        histogram.Observe(10);

        Assert.Throws<ArgumentOutOfRangeException>(() => histogram.Percentile(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => histogram.Percentile(101));
    }

    [Fact]
    public void Reset_ClearsAllObservations()
    {
        var histogram = new Histogram("test");
        histogram.Observe(10);
        histogram.Observe(20);

        histogram.Reset();

        Assert.Equal(0, histogram.Count);
        Assert.Null(histogram.Min);
        Assert.Null(histogram.Max);
    }
}
