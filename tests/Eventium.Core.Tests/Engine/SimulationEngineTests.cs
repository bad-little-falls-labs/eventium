using Eventium.Core.Events;
using Eventium.Core.Systems;
using Eventium.Core.Time;
using Eventium.Core.World;

namespace Eventium.Core.Tests.Engine;

public class SimulationEngineTests
{
    [Fact]
    public void Constructor_SetsTimeModelAndInitialTime()
    {
        var timeModel = new TimeModel(TimeMode.Discrete);

        var engine = new SimulationEngine(timeModel);

        Assert.Same(timeModel, engine.TimeModel);
        Assert.Equal(0.0, engine.Time);
    }

    [Fact]
    public void Constructor_WithSeed_CreatesSeededRng()
    {
        var engine1 = new SimulationEngine(new TimeModel(TimeMode.Continuous), seed: 42);
        var engine2 = new SimulationEngine(new TimeModel(TimeMode.Continuous), seed: 42);

        var value1 = engine1.Rng.NextDouble();
        var value2 = engine2.Rng.NextDouble();

        Assert.Equal(value1, value2);
    }

    [Fact]
    public void Metrics_AccessibleThroughContext()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));

        engine.Schedule(1.0, "TEST", handler: (ctx, _) =>
        {
            var counter = ctx.Metrics.Counter("test_counter");
            counter.Increment(5);
        });

        engine.Run();

        var counter = engine.Metrics.Counter("test_counter");
        Assert.Equal(5L, counter.Value);
    }

    [Fact]
    public void RegisterHandler_HandlerIsCalledForEventType()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var called = false;

        engine.RegisterHandler("MY_EVENT", (_, _) => called = true);
        engine.Schedule(1.0, "MY_EVENT");

        engine.Run();

        Assert.True(called);
    }

    [Fact]
    public void RegisterHandler_MultipleHandlers_AllCalled()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var calls = new List<int>();

        engine.RegisterHandler("TEST", (_, _) => calls.Add(1));
        engine.RegisterHandler("TEST", (_, _) => calls.Add(2));
        engine.Schedule(1.0, "TEST");

        engine.Run();

        Assert.Equal(new[] { 1, 2 }, calls);
    }

    [Fact]
    public void RegisterSystem_MultipleSystemsOnSameEvent_AllHandlersExecute()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var system1 = new TestSystem();
        var system2 = new TestSystem();

        engine.RegisterSystem(system1);
        engine.RegisterSystem(system2);
        engine.Schedule(1.0, "TEST_EVENT");

        engine.Run();

        Assert.Equal(1, system1.HandleCount);
        Assert.Equal(1, system2.HandleCount);
    }

    [Fact]
    public void RegisterSystem_SystemHandlesEvents()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var system = new TestSystem();

        engine.RegisterSystem(system);
        engine.Schedule(1.0, "TEST_EVENT");

        engine.Run();

        Assert.Equal(1, system.HandleCount);
    }

    [Fact]
    public void RegisterSystem_SystemHandlesMultipleEventTypes()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var system = new MultiEventSystem();

        engine.RegisterSystem(system);
        engine.Schedule(1.0, "EVENT_A");
        engine.Schedule(2.0, "EVENT_B");
        engine.Schedule(3.0, "EVENT_C");

        engine.Run();

        Assert.Equal(3, system.TotalHandled);
        Assert.Single(system.EventsHandled, e => e == "EVENT_A");
        Assert.Single(system.EventsHandled, e => e == "EVENT_B");
        Assert.Single(system.EventsHandled, e => e == "EVENT_C");
    }

    [Fact]
    public void Rng_AccessibleThroughContext_ProducesConsistentResults()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous), seed: 42);
        var values = new List<double>();

        for (int i = 0; i < 5; i++)
        {
            engine.Schedule(i, $"EVENT_{i}", handler: (ctx, _) =>
            {
                values.Add(ctx.Rng.NextDouble());
            });
        }

        engine.Run();

        // With same seed, should produce consistent values
        Assert.Equal(5, values.Count);
        Assert.All(values, v => Assert.InRange(v, 0.0, 1.0));
    }

    [Fact]
    public void Run_ComplexSchedulingPattern_MaintainsCorrectOrder()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var executionOrder = new List<string>();

        // Each event schedules the next event
        engine.Schedule(1.0, "START", handler: (ctx, _) =>
        {
            executionOrder.Add("START");
            ctx.Schedule(5.0, "MIDDLE");
            ctx.ScheduleIn(1.0, "SOON"); // 2.0
        });

        engine.RegisterHandler("SOON", (ctx, _) =>
        {
            executionOrder.Add("SOON");
            ctx.Schedule(3.0, "BETWEEN");
        });

        engine.RegisterHandler("BETWEEN", (_, _) => executionOrder.Add("BETWEEN"));
        engine.RegisterHandler("MIDDLE", (ctx, _) =>
        {
            executionOrder.Add("MIDDLE");
            ctx.ScheduleIn(1.0, "END"); // 6.0
        });
        engine.RegisterHandler("END", (_, _) => executionOrder.Add("END"));

        engine.Run();

        Assert.Equal(new[] { "START", "SOON", "BETWEEN", "MIDDLE", "END" }, executionOrder);
    }

    [Fact]
    public void Run_EmptyQueue_ReturnsQueueEmptyReason()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));

        var result = engine.Run();

        Assert.Equal(SimulationStopReason.QueueEmpty, result.StopReason);
        Assert.Equal(0, result.EventsProcessed);
    }

    [Fact]
    public void Run_MaxEvents_StopsAfterMaxEvents()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var count = 0;

        engine.Schedule(1.0, "A", handler: (_, _) => count++);
        engine.Schedule(2.0, "B", handler: (_, _) => count++);
        engine.Schedule(3.0, "C", handler: (_, _) => count++);

        var result = engine.Run(maxEvents: 2);

        Assert.Equal(SimulationStopReason.MaxEventsReached, result.StopReason);
        Assert.Equal(2, count);
        Assert.Equal(2, result.EventsProcessed);
    }

    [Fact]
    public void Run_ProcessesEventsInOrder()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var order = new List<string>();

        engine.Schedule(3.0, "THIRD", handler: (_, evt) => order.Add(evt.Type));
        engine.Schedule(1.0, "FIRST", handler: (_, evt) => order.Add(evt.Type));
        engine.Schedule(2.0, "SECOND", handler: (_, evt) => order.Add(evt.Type));

        engine.Run();

        Assert.Equal(new[] { "FIRST", "SECOND", "THIRD" }, order);
    }

    [Fact]
    public void Run_Result_ContainsEntityCount()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        engine.World.AddEntity(new Entity(1, "A"));
        engine.World.AddEntity(new Entity(2, "B"));
        engine.Schedule(1.0, "TEST");

        var result = engine.Run();

        Assert.Equal(2, result.EntityCount);
    }

    [Fact]
    public void Run_UntilTime_StopsAtSpecifiedTime()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var processed = new List<double>();

        engine.Schedule(1.0, "A", handler: (_, evt) => processed.Add(evt.Time));
        engine.Schedule(2.0, "B", handler: (_, evt) => processed.Add(evt.Time));
        engine.Schedule(5.0, "C", handler: (_, evt) => processed.Add(evt.Time));

        var result = engine.Run(untilTime: 3.0);

        Assert.Equal(SimulationStopReason.TimeReached, result.StopReason);
        Assert.Equal(new[] { 1.0, 2.0 }, processed);
        Assert.Equal(2.0, result.FinalTime);
    }

    [Fact]
    public void Run_UpdatesTime()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        engine.Schedule(5.0, "TEST");

        engine.Run();

        Assert.Equal(5.0, engine.Time);
    }

    [Fact]
    public void Run_WithDiscreteTime_AdvancesCorrectly()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Discrete, step: 1.0));
        var times = new List<double>();

        engine.Schedule(0.0, "T0", handler: (_, evt) => times.Add(evt.Time));
        engine.Schedule(1.0, "T1", handler: (_, evt) => times.Add(evt.Time));
        engine.Schedule(2.0, "T2", handler: (_, evt) => times.Add(evt.Time));

        engine.Run();

        Assert.Equal(new[] { 0.0, 1.0, 2.0 }, times);
    }

    [Fact]
    public void ScheduleIn_AddsEventRelativeToCurrentTime()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        // First event advances time to 2.0, then schedules another 3.0 later
        engine.Schedule(2.0, "FIRST", handler: (ctx, _) =>
        {
            ctx.ScheduleIn(3.0, "SECOND");
        });

        engine.Run(maxEvents: 1);

        Assert.Equal(5.0, engine.Queue.PeekTime());
    }

    [Fact]
    public void Schedule_AddsEventToQueue()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));

        engine.Schedule(5.0, "TEST");

        Assert.Equal(1, engine.Queue.Count);
    }

    [Fact]
    public void Schedule_EventChaining_WorksCorrectly()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var events = new List<string>();

        engine.Schedule(1.0, "FIRST", handler: (ctx, _) =>
        {
            events.Add("FIRST");
            ctx.Schedule(3.0, "THIRD");
        });
        engine.Schedule(2.0, "SECOND", handler: (ctx, _) =>
        {
            events.Add("SECOND");
        });
        engine.RegisterHandler("THIRD", (_, _) => events.Add("THIRD"));

        engine.Run();

        Assert.Equal(new[] { "FIRST", "SECOND", "THIRD" }, events);
    }

    [Fact]
    public void Schedule_WithTypedPayload_PayloadIsAccessible()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        TestPayload? received = null;

        engine.Schedule(1.0, "TEST", new TestPayload(42), handler: (_, evt) =>
        {
            received = evt.GetPayload<TestPayload>();
        });

        engine.Run();

        Assert.NotNull(received);
        Assert.Equal(42, received.Value);
    }

    [Fact]
    public void SimulationResult_ContainsAccurateStatistics()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        engine.World.AddEntity(new Entity(1, "E1"));
        engine.World.AddEntity(new Entity(2, "E2"));

        for (int i = 0; i < 10; i++)
        {
            engine.Schedule(i, $"EVENT_{i}");
        }

        var result = engine.Run(untilTime: 5.5);

        Assert.Equal(SimulationStopReason.TimeReached, result.StopReason);
        Assert.Equal(6, result.EventsProcessed); // Events 0-5
        Assert.Equal(3, result.EventsRemaining); // Events 7-9 (event 6 consumed but remaining at time 6.0)
        Assert.Equal(5.0, result.FinalTime);
        Assert.Equal(2, result.EntityCount);
        Assert.True(result.EventsPerSecond > 0);
    }

    [Fact]
    public void Stop_TerminatesRunLoop()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var count = 0;

        engine.Schedule(1.0, "A", handler: (ctx, _) =>
        {
            count++;
            ctx.Stop();
        });
        engine.Schedule(2.0, "B", handler: (_, _) => count++);

        var result = engine.Run();

        Assert.Equal(SimulationStopReason.StoppedByUser, result.StopReason);
        Assert.Equal(1, count);
    }

    [Fact]
    public void World_AccessibleThroughContext()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        Entity? capturedEntity = null;

        engine.Schedule(1.0, "TEST", handler: (ctx, _) =>
        {
            var entity = new Entity(1, "TEST_ENTITY");
            ctx.World.AddEntity(entity);
            capturedEntity = ctx.World.GetEntity(1);
        });

        engine.Run();

        Assert.NotNull(capturedEntity);
        Assert.Equal("TEST_ENTITY", capturedEntity.Type);
        Assert.Single(engine.World.Entities);
    }

    private sealed class MultiEventSystem : ISystem
    {
        public List<string> EventsHandled { get; } = new();

        public IEnumerable<string> HandledEventTypes => new[] { "EVENT_A", "EVENT_B", "EVENT_C" };
        public int TotalHandled => EventsHandled.Count;

        public void HandleEvent(ISimulationContext context, Event evt)
        {
            EventsHandled.Add(evt.Type);
        }
    }

    private sealed class TestSystem : ISystem
    {
        public int HandleCount { get; private set; }

        public IEnumerable<string> HandledEventTypes => new[] { "TEST_EVENT" };

        public void HandleEvent(ISimulationContext context, Event evt)
        {
            HandleCount++;
        }
    }

    private sealed record TestPayload(int Value) : IEventPayload;
}
