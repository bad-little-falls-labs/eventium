using System.Diagnostics.CodeAnalysis;
using Eventium.Core.Events;
using Eventium.Core.Instrumentation;
using Eventium.Core.Random;
using Eventium.Core.Systems;
using Eventium.Core.Time;

namespace Eventium.Core;

/// <summary>
/// Central orchestrator of a simulation run.
/// </summary>
[SuppressMessage("Design", "S1450:Private fields only used as local variables in methods should become local variables", Justification = "_running is set by Stop() to halt Run() loop")]
public sealed class SimulationEngine
{
    private readonly Dictionary<string, List<EventHandlerDelegate>> _handlers = new();
    private readonly List<ISystem> _systems = new();
    private bool _running;

    public TimeModel TimeModel { get; }
    public double Time { get; private set; }

    public World.World World { get; } = new();
    public EventQueue Queue { get; } = new();
    public IRandomSource Rng { get; }
    public MetricsRegistry Metrics { get; } = new();

    public SimulationEngine(TimeModel timeModel, int? seed = null)
    {
        TimeModel = timeModel;
        Time = Eventium.Core.Time.TimeModel.InitialTime;
        Rng = new DefaultRandomSource(seed);
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

    public void RegisterHandler(string eventType, EventHandlerDelegate handler)
    {
        if (!_handlers.TryGetValue(eventType, out var list))
        {
            list = new List<EventHandlerDelegate>();
            _handlers[eventType] = list;
        }

        list.Add(handler);
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

    public void ScheduleIn(
        double dt,
        string type,
        IDictionary<string, object?>? payload = null,
        int priority = 0,
        EventHandlerDelegate? handler = null)
    {
        Schedule(Time + dt, type, payload, priority, handler);
    }

    // --- Running ---

    public void Run(double? untilTime = null, int? maxEvents = null)
    {
        _running = true;
        var processed = 0;

        while (_running && Queue.Count > 0)
        {
            var evt = Queue.Dequeue();
            if (evt is null) break;

            if (untilTime.HasValue && evt.Time > untilTime.Value)
            {
                // NOTE: in a future version we may re-enqueue and exit.
                break;
            }

            Time = evt.Time;
            evt.Handler(this, evt);

            processed++;
            if (maxEvents.HasValue && processed >= maxEvents.Value)
                break;
        }
    }

    public void Stop()
    {
        _running = false;
    }

    // --- Internal dispatch ---

    private void DispatchEvent(SimulationEngine engine, Event evt)
    {
        if (_handlers.TryGetValue(evt.Type, out var list))
        {
            foreach (var handler in list)
            {
                handler(engine, evt);
            }
        }
    }
}
