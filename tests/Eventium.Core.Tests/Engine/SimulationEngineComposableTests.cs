// <copyright file="SimulationEngineComposableTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core;
using Eventium.Core.Time;
using Xunit;

namespace Eventium.Core.Tests.Engine;

/// <summary>
/// Tests for the composable simulation stepping API.
/// </summary>
public sealed class SimulationEngineComposableTests
{

    [Fact]
    public void ProcessUntil_CanBeCalledMultipleTimes()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var totalEventsProcessed = 0;

        for (int i = 0; i < 10; i++)
        {
            engine.Schedule(i, "TICK");
        }

        // Process first 3 events
        var result1 = engine.ProcessUntil(maxEvents: 3);
        totalEventsProcessed += result1.EventsProcessed;
        Assert.Equal(3, result1.EventsProcessed);
        Assert.Equal(7, result1.EventsRemaining);

        // Process next 2 events
        var result2 = engine.ProcessUntil(maxEvents: 2);
        totalEventsProcessed += result2.EventsProcessed;
        Assert.Equal(2, result2.EventsProcessed);
        Assert.Equal(5, result2.EventsRemaining);

        // Process remaining
        var result3 = engine.ProcessUntil();
        totalEventsProcessed += result3.EventsProcessed;

        Assert.Equal(10, totalEventsProcessed);
        Assert.Equal(0, result3.EventsRemaining);
        Assert.Equal(SimulationStopReason.QueueEmpty, result3.StopReason);
    }

    [Fact]
    public void ProcessUntil_ProcessesMultipleEvents()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var eventsProcessed = 0;

        engine.RegisterHandler("TICK", (context, evt) =>
        {
            eventsProcessed++;
        });

        for (int i = 0; i < 5; i++)
        {
            engine.Schedule(i, "TICK");
        }

        var result = engine.ProcessUntil(maxEvents: 3);

        Assert.Equal(3, result.EventsProcessed);
        Assert.Equal(2, result.EventsRemaining);
        Assert.Equal(2.0, result.FinalTime);
        Assert.Equal(SimulationStopReason.MaxEventsReached, result.StopReason);
    }

    [Fact]
    public void ProcessUntil_StopsAtTime()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));

        for (int i = 0; i < 10; i++)
        {
            engine.Schedule(i, "TICK");
        }

        var result = engine.ProcessUntil(untilTime: 4.5);

        Assert.Equal(5, result.EventsProcessed);
        Assert.Equal(4.0, result.FinalTime);
        Assert.Equal(SimulationStopReason.TimeReached, result.StopReason);
    }

    [Fact]
    public void Run_ConvenienceWrapperUsesMethods()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));

        for (int i = 0; i < 5; i++)
        {
            engine.Schedule(i, "TICK");
        }

        var result = engine.Run(maxEvents: 3);

        Assert.Equal(3, result.EventsProcessed);
        Assert.Equal(2, result.EventsRemaining);
        Assert.IsType<SimulationResult>(result);
    }
    [Fact]
    public void TryProcessNextEvent_ProcessesSingleEvent()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Discrete));
        var eventFired = false;

        engine.RegisterHandler("TEST", (context, evt) =>
        {
            eventFired = true;
        });

        engine.Schedule(0, "TEST");

        var success = engine.TryProcessNextEvent(out var processed);

        Assert.True(success);
        Assert.NotNull(processed);
        Assert.Equal("TEST", processed.Type);
        Assert.True(eventFired);
    }

    [Fact]
    public void TryProcessNextEvent_ReturnsFalseWhenQueueEmpty()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Discrete));

        var success = engine.TryProcessNextEvent(out var processed);

        Assert.False(success);
        Assert.Null(processed);
    }
}
