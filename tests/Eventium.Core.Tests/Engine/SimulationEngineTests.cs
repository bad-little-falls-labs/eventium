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
