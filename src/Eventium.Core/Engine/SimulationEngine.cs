using System.Diagnostics.CodeAnalysis;
using Eventium.Core.Events;
using Eventium.Core.Instrumentation;
using Eventium.Core.Random;
using Eventium.Core.Systems;
using Eventium.Core.Time;
using Eventium.Core.World;

namespace Eventium.Core;

/// <summary>
/// Central orchestrator of a simulation run.
/// </summary>
[SuppressMessage("Design", "S1450:Private fields only used as local variables in methods should become local variables", Justification = "_running is set by Stop() to halt Run() loop")]
public sealed class SimulationEngine : ISimulationEngine
{
    private readonly Dictionary<string, List<EventHandlerDelegate>> _handlers = new();
    private readonly List<ISystem> _systems = new();
    private bool _running;

    public SimulationEngine(TimeModel timeModel, int? seed = null)
        : this(timeModel, new World.World(), new EventQueue(), new DefaultRandomSource(seed))
    {
    }

    public SimulationEngine(TimeModel timeModel, IWorld world, IEventQueue queue, IRandomSource rng)
    {
        TimeModel = timeModel;
        Time = Eventium.Core.Time.TimeModel.InitialTime;
        World = world;
        Queue = queue;
        Rng = rng;
    }
    public MetricsRegistry Metrics { get; } = new();
    public IEventQueue Queue { get; }
    public IRandomSource Rng { get; }
    public double Time { get; private set; }

    public TimeModel TimeModel { get; }

    public IWorld World { get; }

    public void RegisterHandler(string eventType, EventHandlerDelegate handler)
    {
        if (!_handlers.TryGetValue(eventType, out var list))
        {
            list = new List<EventHandlerDelegate>();
            _handlers[eventType] = list;
        }

        list.Add(handler);
    }

    // --- System registration ---

    public void RegisterSystem(ISystem system)
    {
        _systems.Add(system);
        foreach (var type in system.HandledEventTypes)
        {
            if (!_handlers.TryGetValue(type, out var list))
            {
                list = new List<EventHandlerDelegate>();
                _handlers[type] = list;
            }

            list.Add(system.HandleEvent);
        }
    }

    // --- Running ---

    public SimulationResult Run(double? untilTime = null, int? maxEvents = null)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _running = true;
        var processed = 0;
        var stopReason = SimulationStopReason.QueueEmpty;

        while (_running && Queue.Count > 0)
        {
            var evt = Queue.Dequeue();
            if (evt is null)
                break;

            if (untilTime.HasValue && evt.Time > untilTime.Value)
            {
                stopReason = SimulationStopReason.TimeReached;
                break;
            }

            Time = evt.Time;
            evt.Handler(this, evt);

            processed++;
            if (maxEvents.HasValue && processed >= maxEvents.Value)
            {
                stopReason = SimulationStopReason.MaxEventsReached;
                break;
            }
        }

        stopwatch.Stop();

        if (!_running)
        {
            stopReason = SimulationStopReason.StoppedByUser;
        }

        return new SimulationResult(
            stopReason: stopReason,
            finalTime: Time,
            eventsProcessed: processed,
            eventsRemaining: Queue.Count,
            wallClockDuration: stopwatch.Elapsed,
            entityCount: World.Entities.Count);
    }

    // --- Scheduling ---

    public void Schedule(
        double time,
        string type,
        IDictionary<string, object?>? payload = null,
        int priority = 0,
        EventHandlerDelegate? handler = null)
    {
        var effectiveHandler = handler ?? DispatchEvent;
        var evt = new Event(time, priority, type, payload, effectiveHandler);
        Queue.Enqueue(evt);
    }

    public void Schedule<TPayload>(
        double time,
        string type,
        TPayload payload,
        int priority = 0,
        EventHandlerDelegate? handler = null) where TPayload : IEventPayload
    {
        var effectiveHandler = handler ?? DispatchEvent;
        var evt = new Event(time, priority, type, payload, effectiveHandler);
        Queue.Enqueue(evt);
    }

    public void ScheduleIn(
        double dt,
        string type,
        IDictionary<string, object?>? payload = null,
        int priority = 0,
        EventHandlerDelegate? handler = null)
    {
        Schedule(Time + dt, type, payload, priority, handler);
    }

    public void ScheduleIn<TPayload>(
        double dt,
        string type,
        TPayload payload,
        int priority = 0,
        EventHandlerDelegate? handler = null) where TPayload : IEventPayload
    {
        Schedule(Time + dt, type, payload, priority, handler);
    }

    public void Stop()
    {
        _running = false;
    }

    // --- Internal dispatch ---

    private void DispatchEvent(ISimulationContext context, Event evt)
    {
        if (_handlers.TryGetValue(evt.Type, out var list))
        {
            foreach (var handler in list)
            {
                handler(context, evt);
            }
        }
    }
}
