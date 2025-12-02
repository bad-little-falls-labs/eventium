// <copyright file="DefaultRandomSourceTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.Random;

namespace Eventium.Core.Tests.Random;

public class DefaultRandomSourceTests
{

    [Fact]
    public void Constructor_WithSeed_CreatesSeededRandomSource()
    {
        var rng = new DefaultRandomSource(42);

        Assert.NotNull(rng);
    }
    [Fact]
    public void Constructor_WithoutSeed_CreatesRandomSource()
    {
        var rng = new DefaultRandomSource();

        Assert.NotNull(rng);
    }

    [Fact]
    public void NextDouble_ReturnsValueBetweenZeroAndOne()
    {
        var rng = new DefaultRandomSource();

        for (int i = 0; i < 100; i++)
        {
            var value = rng.NextDouble();
            Assert.InRange(value, 0.0, 1.0);
        }
    }

    [Fact]
    public void NextDouble_WithDifferentSeeds_GeneratesDifferentSequences()
    {
        var rng1 = new DefaultRandomSource(42);
        var rng2 = new DefaultRandomSource(43);

        var values1 = new List<double>();
        var values2 = new List<double>();

        for (int i = 0; i < 10; i++)
        {
            values1.Add(rng1.NextDouble());
            values2.Add(rng2.NextDouble());
        }

        Assert.NotEqual(values1, values2);
    }

    [Fact]
    public void NextDouble_WithSameSeed_GeneratesSameSequence()
    {
        var rng1 = new DefaultRandomSource(42);
        var rng2 = new DefaultRandomSource(42);

        var values1 = new List<double>();
        var values2 = new List<double>();

        for (int i = 0; i < 10; i++)
        {
            values1.Add(rng1.NextDouble());
            values2.Add(rng2.NextDouble());
        }

        Assert.Equal(values1, values2);
    }

    [Fact]
    public void NextInt_MinEqualsMax_ReturnsMin()
    {
        var rng = new DefaultRandomSource();

        var value = rng.NextInt(5, 5);

        Assert.Equal(5, value);
    }

    [Fact]
    public void NextInt_ReturnsValueWithinRange()
    {
        var rng = new DefaultRandomSource();

        for (int i = 0; i < 100; i++)
        {
            var value = rng.NextInt(1, 10);
            Assert.InRange(value, 1, 9);
        }
    }

    [Fact]
    public void NextInt_SingleValueRange_ReturnsMin()
    {
        var rng = new DefaultRandomSource();

        var value = rng.NextInt(0, 1);

        Assert.Equal(0, value);
    }

    [Fact]
    public void NextInt_WithMaxValue_ReturnsValueLessThanMax()
    {
        var rng = new DefaultRandomSource();

        for (int i = 0; i < 100; i++)
        {
            var value = rng.NextInt(0, 100);
            Assert.InRange(value, 0, 99);
        }
    }

    [Fact]
    public void NextInt_WithSameSeed_GeneratesSameSequence()
    {
        var rng1 = new DefaultRandomSource(42);
        var rng2 = new DefaultRandomSource(42);

        var values1 = new List<int>();
        var values2 = new List<int>();

        for (int i = 0; i < 10; i++)
        {
            values1.Add(rng1.NextInt(0, 100));
            values2.Add(rng2.NextInt(0, 100));
        }

        Assert.Equal(values1, values2);
    }

    [Theory]
    [InlineData(42)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(12345)]
    public void SeedReproducibility_VariousSeeds_ProducesSameSequence(int seed)
    {
        var rng1 = new DefaultRandomSource(seed);
        var rng2 = new DefaultRandomSource(seed);

        for (int i = 0; i < 20; i++)
        {
            Assert.Equal(rng1.NextDouble(), rng2.NextDouble());
            Assert.Equal(rng1.NextInt(0, 1000), rng2.NextInt(0, 1000));
        }
    }
}
