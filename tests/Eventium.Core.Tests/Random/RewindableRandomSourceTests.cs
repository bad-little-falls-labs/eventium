// <copyright file="RewindableRandomSourceTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.Random;
using Xunit;

namespace Eventium.Core.Tests.Random;

/// <summary>
/// Tests for rewindable random number generator with state capture/restoration.
/// </summary>
public sealed class RewindableRandomSourceTests
{
    [Fact]
    public void RewindableRandomSource_IsDeterministicWithSameSeed()
    {
        var rng1 = new RewindableRandomSource(42);
        var rng2 = new RewindableRandomSource(42);

        for (int i = 0; i < 100; i++)
        {
            Assert.Equal(rng1.NextDouble(), rng2.NextDouble());
        }
    }

    [Fact]
    public void RewindableRandomSource_ProducesDifferentSequencesWithDifferentSeeds()
    {
        var rng1 = new RewindableRandomSource(42);
        var rng2 = new RewindableRandomSource(43);

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
    public void RewindableRandomSource_NextDouble_IsInRange()
    {
        var rng = new RewindableRandomSource(42);

        for (int i = 0; i < 1000; i++)
        {
            var value = rng.NextDouble();
            Assert.True(value >= 0.0 && value < 1.0);
        }
    }

    [Fact]
    public void RewindableRandomSource_NextInt_IsInRange()
    {
        var rng = new RewindableRandomSource(42);

        for (int i = 0; i < 1000; i++)
        {
            var value = rng.NextInt(10, 20);
            Assert.True(value >= 10 && value < 20);
        }
    }

    [Fact]
    public void RewindableRandomSource_NextInt_RejectsInvalidRange()
    {
        var rng = new RewindableRandomSource(42);

        var ex = Assert.Throws<ArgumentException>(() => rng.NextInt(20, 10));
        Assert.Contains("must be less than", ex.Message);
    }

    [Fact]
    public void RewindableRandomSource_GetState_ReturnsStateObject()
    {
        var rng = new RewindableRandomSource(42);

        // Advance RNG
        _ = rng.NextDouble();
        _ = rng.NextDouble();

        var state = rng.GetState();

        Assert.NotNull(state);
    }

    [Fact]
    public void RewindableRandomSource_SetState_RestoresSequence()
    {
        var rng = new RewindableRandomSource(42);

        // Get initial values
        var value1 = rng.NextDouble();
        var value2 = rng.NextDouble();

        // Capture state after 2 values
        var state = rng.GetState();

        // Generate more values
        var value3 = rng.NextDouble();
        var value4 = rng.NextDouble();

        // Restore to saved state
        rng.SetState(state);

        // Should get the same values as before
        Assert.Equal(value3, rng.NextDouble());
        Assert.Equal(value4, rng.NextDouble());
    }

    [Fact]
    public void RewindableRandomSource_StateSnapshot_IsIndependent()
    {
        var rng = new RewindableRandomSource(42);

        var value1 = rng.NextDouble();
        var state = rng.GetState();
        var value2 = rng.NextDouble();

        // Create a second RNG and set it to the captured state
        var rng2 = new RewindableRandomSource(999);
        rng2.SetState(state);

        // Should produce the same sequence as the original
        Assert.Equal(value2, rng2.NextDouble());
    }

    [Fact]
    public void RewindableRandomSource_SetState_RejectsInvalidState()
    {
        var rng = new RewindableRandomSource(42);

        var ex = Assert.Throws<ArgumentException>(() => rng.SetState("invalid"));

        Assert.Contains("RngStateSnapshot", ex.Message);
    }

    [Fact]
    public void RewindableRandomSource_MultipleCaptures_AllWork()
    {
        var rng = new RewindableRandomSource(42);

        var states = new List<object>();

        for (int i = 0; i < 5; i++)
        {
            states.Add(rng.GetState());
            _ = rng.NextDouble();
        }

        // Restore to each state and verify
        for (int i = states.Count - 1; i >= 0; i--)
        {
            rng.SetState(states[i]);

            // Should be able to get the next value
            var nextValue = rng.NextDouble();
            Assert.True(nextValue >= 0.0 && nextValue < 1.0);
        }
    }

    [Fact]
    public void RewindableRandomSource_Implements_IRandomSourceWithState()
    {
        var rng = new RewindableRandomSource(42);

        Assert.IsAssignableFrom<IRandomSourceWithState>(rng);
        Assert.IsAssignableFrom<IRandomSource>(rng);
    }

    [Fact]
    public void RewindableRandomSource_ProducesDiverseValues()
    {
        var rng = new RewindableRandomSource(42);
        var values = new HashSet<double>();

        for (int i = 0; i < 1000; i++)
        {
            values.Add(rng.NextDouble());
        }

        // Should have many unique values (not all duplicates or constant)
        Assert.True(values.Count > 900);
    }

    [Fact]
    public void RewindableRandomSource_HandlesLargeRange()
    {
        var rng = new RewindableRandomSource(42);

        for (int i = 0; i < 100; i++)
        {
            var value = rng.NextInt(0, 1000000);
            Assert.True(value >= 0 && value < 1000000);
        }
    }
}
