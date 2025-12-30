// <copyright file="SimulationClockTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.Time;
using Xunit;

namespace Eventium.Core.Tests.Time;

/// <summary>
/// Tests for SimulationClock time control functionality.
/// </summary>
public sealed class SimulationClockTests
{
    [Fact]
    public void SimulationClock_InitializesWithDefaults()
    {
        var clock = new SimulationClock(TimeMode.Continuous);

        Assert.Equal(TimeMode.Continuous, clock.Mode);
        Assert.Equal(StepPolicy.Event, clock.StepPolicy);
        Assert.Equal(1.0, clock.TimeScale);
        Assert.False(clock.IsPaused);
    }

    [Fact]
    public void SimulationClock_InitializesWithCustomPolicy()
    {
        var clock = new SimulationClock(TimeMode.Discrete, StepPolicy.Tick);

        Assert.Equal(TimeMode.Discrete, clock.Mode);
        Assert.Equal(StepPolicy.Tick, clock.StepPolicy);
    }

    [Fact]
    public void IsPaused_CanBeToggled()
    {
        var clock = new SimulationClock(TimeMode.Continuous);

        Assert.False(clock.IsPaused);

        clock.IsPaused = true;
        Assert.True(clock.IsPaused);

        clock.IsPaused = false;
        Assert.False(clock.IsPaused);
    }

    [Fact]
    public void TimeScale_CanBeChanged()
    {
        var clock = new SimulationClock(TimeMode.Continuous);

        clock.TimeScale = 2.0;
        Assert.Equal(2.0, clock.TimeScale);

        clock.TimeScale = 0.5;
        Assert.Equal(0.5, clock.TimeScale);
    }

    [Fact]
    public void TimeScale_RejectsZeroOrNegative()
    {
        var clock = new SimulationClock(TimeMode.Continuous);

        var ex1 = Assert.Throws<ArgumentException>(() => clock.TimeScale = 0);
        Assert.Contains("greater than 0", ex1.Message);

        var ex2 = Assert.Throws<ArgumentException>(() => clock.TimeScale = -1.0);
        Assert.Contains("greater than 0", ex2.Message);
    }

    [Fact]
    public void StepPolicy_CanBeChanged()
    {
        var clock = new SimulationClock(TimeMode.Continuous, StepPolicy.Event);

        Assert.Equal(StepPolicy.Event, clock.StepPolicy);

        clock.StepPolicy = StepPolicy.Tick;
        Assert.Equal(StepPolicy.Tick, clock.StepPolicy);

        clock.StepPolicy = StepPolicy.Turn;
        Assert.Equal(StepPolicy.Turn, clock.StepPolicy);
    }

    [Theory]
    [InlineData(TimeMode.Discrete, StepPolicy.Event, 1.0, false)]
    [InlineData(TimeMode.Continuous, StepPolicy.Tick, 2.0, true)]
    [InlineData(TimeMode.Discrete, StepPolicy.Turn, 0.5, false)]
    public void SimulationClock_SupportsAllConfigurations(TimeMode mode, StepPolicy policy, double scale, bool paused)
    {
        var clock = new SimulationClock(mode, policy)
        {
            TimeScale = scale,
            IsPaused = paused
        };

        Assert.Equal(mode, clock.Mode);
        Assert.Equal(policy, clock.StepPolicy);
        Assert.Equal(scale, clock.TimeScale);
        Assert.Equal(paused, clock.IsPaused);
    }

    [Fact]
    public void ToString_ReturnsDescriptiveString()
    {
        var clock = new SimulationClock(TimeMode.Continuous, StepPolicy.Tick)
        {
            TimeScale = 2.5,
            IsPaused = true
        };

        var str = clock.ToString();

        Assert.Contains("Mode=Continuous", str);
        Assert.Contains("Policy=Tick", str);
        Assert.Contains("TimeScale=2.50x", str);
        Assert.Contains("PAUSED", str);
    }

    [Fact]
    public void ToString_ShowsRunningWhenNotPaused()
    {
        var clock = new SimulationClock(TimeMode.Discrete);

        var str = clock.ToString();

        Assert.Contains("RUNNING", str);
    }
}
