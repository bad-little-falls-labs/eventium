// <copyright file="CounterTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.Instrumentation;

namespace Eventium.Core.Tests.Instrumentation;

public class CounterTests
{
    [Fact]
    public void Constructor_SetsNameAndInitialValueToZero()
    {
        var counter = new Counter("test_counter");

        Assert.Equal("test_counter", counter.Name);
        Assert.Equal(0L, counter.Value);
    }

    [Fact]
    public void Increment_Multiple_Accumulates()
    {
        var counter = new Counter("test");

        counter.Increment();
        counter.Increment(3);
        counter.Increment(2);

        Assert.Equal(6L, counter.Value);
    }

    [Fact]
    public void Increment_WithAmount_IncrementsCorrectly()
    {
        var counter = new Counter("test");

        counter.Increment(5);

        Assert.Equal(5L, counter.Value);
    }

    [Fact]
    public void Increment_WithNegative_Decrements()
    {
        var counter = new Counter("test");
        counter.Increment(10);

        counter.Increment(-3);

        Assert.Equal(7L, counter.Value);
    }

    [Fact]
    public void Increment_WithZero_DoesNotChange()
    {
        var counter = new Counter("test");
        counter.Increment(5);

        counter.Increment(0);

        Assert.Equal(5L, counter.Value);
    }

    [Fact]
    public void Increment_WithoutAmount_IncrementsBy1()
    {
        var counter = new Counter("test");

        counter.Increment();

        Assert.Equal(1L, counter.Value);
    }

    [Fact]
    public void Value_StartAtZero_RemainsZeroUntilIncremented()
    {
        var counter = new Counter("test");

        Assert.Equal(0L, counter.Value);
        counter.Increment();
        Assert.Equal(1L, counter.Value);
    }
}
