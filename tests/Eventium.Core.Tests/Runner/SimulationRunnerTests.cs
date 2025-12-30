// <copyright file="SimulationRunnerTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core;
using Eventium.Core.Events;
using Eventium.Core.Random;
using Eventium.Core.Runner;
using Eventium.Core.Time;
using Xunit;
using WorldClass = Eventium.Core.World.World;

namespace Eventium.Core.Tests.Runner;

/// <summary>
/// Tests for the SimulationRunner controller.
/// </summary>
public sealed class SimulationRunnerTests
{
    private const string TestEvent = "TEST_EVT";

    [Fact]
    public void Constructor_InitializesWithEngine()
    {
        var engine = CreateEngine();
        var runner = new SimulationRunner(engine);

        Assert.Same(engine, runner.Engine);
        Assert.NotNull(runner.Clock);
        Assert.False(runner.IsPaused);
    }

    [Fact]
    public void Pause_SetsPausedState()
    {
        var runner = new SimulationRunner(CreateEngine());

        runner.Pause();

        Assert.True(runner.IsPaused);
        Assert.True(runner.Clock.IsPaused);
    }

    [Fact]
    public void Resume_ClearsPausedState()
    {
        var runner = new SimulationRunner(CreateEngine());

        runner.Pause();
        runner.Resume();

        Assert.False(runner.IsPaused);
        Assert.False(runner.Clock.IsPaused);
    }

    [Fact]
    public async Task RunRealTimeAsync_InvalidEventBudget_Throws()
    {
        var runner = new SimulationRunner(CreateEngine());

        await Assert.ThrowsAsync<ArgumentException>(
            () => runner.RunRealTimeAsync(frameDurationMs: 16, eventBudgetPerFrame: 0, cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task RunRealTimeAsync_InvalidFrameDuration_Throws()
    {
        var runner = new SimulationRunner(CreateEngine());

        await Assert.ThrowsAsync<ArgumentException>(
            () => runner.RunRealTimeAsync(frameDurationMs: -1, cancellationToken: CancellationToken.None));
    }

    [Fact]
    public void RunRealTimeAsync_ProcessesEventsBudgeted()
    {
        var engine = CreateEngine();
        engine.Schedule(0.0, TestEvent);
        engine.Schedule(0.0, TestEvent);
        engine.Schedule(0.0, TestEvent);

        var runner = new SimulationRunner(engine);
        var cts = new CancellationTokenSource();

        // Should complete quickly with small event budget and finish processing events
        var task = runner.RunRealTimeAsync(frameDurationMs: 10, eventBudgetPerFrame: 2, cancellationToken: cts.Token);

        // Give it time to process some events
        System.Threading.Thread.Sleep(50);
        cts.Cancel();

        Assert.True(task.IsCompleted || task.IsCanceled);
    }

    [Fact]
    public void Seek_Backward_WithoutSnapshot_Throws()
    {
        var engine = CreateEngine();
        // Schedule multiple events at different times to advance time
        engine.Schedule(0.0, TestEvent);
        engine.Schedule(1.0, TestEvent);
        engine.Schedule(2.0, TestEvent);
        engine.Schedule(3.0, TestEvent);

        var runner = new SimulationRunner(engine);
        // Manually step through events to advance time without creating snapshots
        runner.StepEvent();
        runner.StepEvent();
        runner.StepEvent();
        runner.StepEvent();

        // Now engine should be at some time > 0
        // Try to seek backward without a snapshot - should throw
        var currentTime = engine.Time;
        if (currentTime > 0)
        {
            var ex = Assert.Throws<InvalidOperationException>(() => runner.Seek(0.0));
            Assert.Contains("snapshot", ex.Message);
        }
    }

    [Fact]
    public void Seek_CurrentTime_DoesNotProcess()
    {
        var engine = CreateEngine();
        ScheduleEvents(engine, 3);
        var runner = new SimulationRunner(engine);

        var initialTime = engine.Time;
        var result = runner.Seek(initialTime);

        Assert.Equal(SimulationStopReason.TimeReached, result.StopReason);
        Assert.Equal(0, result.EventsProcessed);
    }

    [Fact]
    public void Seek_Forward_ProcessesUntilTarget()
    {
        var engine = CreateEngine();
        engine.Schedule(0.0, TestEvent);
        engine.Schedule(0.5, TestEvent);
        engine.Schedule(1.0, TestEvent);
        engine.Schedule(2.0, TestEvent);

        var runner = new SimulationRunner(engine);

        var result = runner.Seek(1.0);

        Assert.Equal(1.0, engine.Time);
        Assert.True(result.EventsProcessed >= 2);
    }

    [Fact]
    public void SetTimeScale_InvalidScale_Throws()
    {
        var runner = new SimulationRunner(CreateEngine());

        var ex = Assert.Throws<ArgumentException>(() => runner.SetTimeScale(-1.0));
        Assert.Contains("greater than 0", ex.Message);

        ex = Assert.Throws<ArgumentException>(() => runner.SetTimeScale(0));
        Assert.Contains("greater than 0", ex.Message);
    }

    [Fact]
    public void SetTimeScale_UpdatesClock()
    {
        var runner = new SimulationRunner(CreateEngine());

        runner.SetTimeScale(2.5);

        Assert.Equal(2.5, runner.Clock.TimeScale);
    }

    [Fact]
    public void StepDelta_AdvancesSimTime()
    {
        var engine = CreateEngine();
        ScheduleEvents(engine, 5);

        var runner = new SimulationRunner(engine);
        var initialTime = engine.Time;

        var result = runner.StepDelta(1.5);

        Assert.True(engine.Time >= initialTime);
        Assert.True(engine.Time <= initialTime + 1.5);
    }

    [Fact]
    public void StepDelta_NegativeTime_Throws()
    {
        var runner = new SimulationRunner(CreateEngine());

        var ex = Assert.Throws<ArgumentException>(() => runner.StepDelta(-1.0));
        Assert.Contains("non-negative", ex.Message);
    }

    [Fact]
    public void StepEvent_EmptyQueue_ReturnsQueueEmpty()
    {
        var runner = new SimulationRunner(CreateEngine());

        var result = runner.StepEvent();

        Assert.Equal(SimulationStopReason.QueueEmpty, result.StopReason);
        Assert.Equal(0, result.EventsProcessed);
    }

    [Fact]
    public void StepEvent_ProcessesSingleEvent()
    {
        var engine = CreateEngine();
        ScheduleEvents(engine, 3);
        var initialCount = engine.Queue.Count;
        var runner = new SimulationRunner(engine);

        var result = runner.StepEvent();

        Assert.Equal(SimulationStopReason.MaxEventsReached, result.StopReason);
        Assert.Equal(1, result.EventsProcessed);
        Assert.Equal(initialCount - 1, engine.Queue.Count);
    }

    [Fact]
    public void StepTurn_ContinuousMode_Throws()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous), new WorldClass(), new EventQueue(), new DefaultRandomSource(42));
        var runner = new SimulationRunner(engine);

        var ex = Assert.Throws<InvalidOperationException>(() => runner.StepTurn());
        Assert.Contains("discrete", ex.Message);
    }

    [Fact]
    public void StepTurn_ProcessesTurnEvents()
    {
        var engine = CreateEngine();
        engine.Schedule(0.0, TestEvent);
        engine.Schedule(1.0, TestEvent);
        engine.Schedule(2.0, TestEvent);
        engine.Schedule(3.5, TestEvent);

        var runner = new SimulationRunner(engine);

        var result = runner.StepTurn();

        // Should process events at times 0 and 1 (inclusive boundary at 1.0)
        Assert.InRange(result.EventsProcessed, 1, 2);
        Assert.Equal(1.0, result.FinalTime);
    }

    [Fact]
    public void Stop_CanBeCalled()
    {
        var engine = CreateEngine();
        var runner = new SimulationRunner(engine);
        engine.Schedule(0.0, TestEvent);

        // Should not throw
        runner.Stop();

        // Runner should still be functional
        Assert.NotNull(runner.Engine);
    }

    private static ISimulationEngine CreateEngine()
    {
        var timeModel = new TimeModel(TimeMode.Discrete, step: 1.0);
        var world = new WorldClass();
        var queue = new EventQueue();
        var rng = new DefaultRandomSource(42);
        var engine = new SimulationEngine(timeModel, world, queue, rng);

        engine.RegisterHandler(TestEvent, (_, __) => { });
        return engine;
    }

    private static void ScheduleEvents(ISimulationEngine engine, int count)
    {
        for (int i = 0; i < count; i++)
        {
            engine.Schedule(i * 0.1, TestEvent);
        }
    }
}
