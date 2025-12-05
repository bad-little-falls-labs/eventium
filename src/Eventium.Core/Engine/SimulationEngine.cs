// <copyright file="SimulationEngine.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationEngine"/> class with default components.
    /// </summary>
    /// <param name="timeModel">The time model (discrete or continuous).</param>
    /// <param name="seed">Optional seed for the random number generator.</param>
    public SimulationEngine(TimeModel timeModel, int? seed = null)
        : this(timeModel, new World.World(), new EventQueue(), new DefaultRandomSource(seed))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationEngine"/> class with custom components.
    /// </summary>
    /// <param name="timeModel">The time model (discrete or continuous).</param>
    /// <param name="world">The world for entity-component management.</param>
    /// <param name="queue">The event queue for scheduling.</param>
    /// <param name="rng">The random number generator.</param>
    public SimulationEngine(TimeModel timeModel, IWorld world, IEventQueue queue, IRandomSource rng)
    {
        TimeModel = timeModel;
        Time = Eventium.Core.Time.TimeModel.InitialTime;
        World = world;
        Queue = queue;
        Rng = rng;
    }

    /// <summary>
    /// Gets the metrics registry for instrumentation.
    /// </summary>
    public MetricsRegistry Metrics { get; } = new();

    /// <summary>
    /// Gets the event queue.
    /// </summary>
    public IEventQueue Queue { get; }

    /// <summary>
    /// Gets the random number generator.
    /// </summary>
    public IRandomSource Rng { get; }

    /// <summary>
    /// Gets the current simulation time.
    /// </summary>
    public double Time { get; private set; }

    /// <summary>
    /// Gets the time model (discrete or continuous).
    /// </summary>
    public TimeModel TimeModel { get; }

    /// <summary>
    /// Gets the simulated world.
    /// </summary>
    public IWorld World { get; }

    /// <summary>
    /// Registers a handler for a specific event type.
    /// </summary>
    /// <param name="eventType">The event type identifier to handle.</param>
    /// <param name="handler">The handler delegate to invoke for this event type.</param>
    public void RegisterHandler(string eventType, EventHandlerDelegate handler)
    {
        if (!_handlers.TryGetValue(eventType, out var list))
        {
            list = new List<EventHandlerDelegate>();
            _handlers[eventType] = list;
        }

        list.Add(handler);
    }

    /// <summary>
    /// Registers a system to handle events.
    /// </summary>
    /// <param name="system">The system to register for handling its declared event types.</param>
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

    /// <summary>
    /// Runs the simulation until a condition is met.
    /// </summary>
    /// <param name="untilTime">Optional maximum simulation time.</param>
    /// <param name="maxEvents">Optional maximum number of events to process.</param>
    /// <returns>A result containing statistics about the simulation run.</returns>
    public SimulationResult Run(double? untilTime = null, int? maxEvents = null)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var eventsProcessedCounter = Metrics.GetCounter("sim.events_processed");
        var queueSizeGauge = Metrics.GetGauge("sim.queue_size");
        var simTimeGauge = Metrics.GetGauge("sim.time");

        _running = true;
        var processed = 0;
        var stopReason = SimulationStopReason.QueueEmpty;

        while (_running && Queue.Count > 0)
        {
            queueSizeGauge.Set(Queue.Count);

            var evt = Queue.Dequeue();
            if (evt is null)
                break;

            if (untilTime.HasValue && evt.Time > untilTime.Value)
            {
                stopReason = SimulationStopReason.TimeReached;
                break;
            }

            Time = evt.Time;
            simTimeGauge.Set(Time);

            evt.Handler(this, evt);

            processed++;
            eventsProcessedCounter.Increment();

            if (maxEvents.HasValue && processed >= maxEvents.Value)
            {
                stopReason = SimulationStopReason.MaxEventsReached;
                break;
            }
        }

        stopwatch.Stop();
        queueSizeGauge.Set(Queue.Count);

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

    /// <summary>
    /// Schedules an event at an absolute time.
    /// </summary>
    /// <param name="time">The absolute simulation time for the event.</param>
    /// <param name="type">The event type identifier.</param>
    /// <param name="payload">Optional dictionary payload for event data.</param>
    /// <param name="priority">The priority for ordering events at the same time (default 0).</param>
    /// <param name="handler">Optional handler override; uses registered handler if null.</param>
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

    /// <summary>
    /// Schedules an event at an absolute time with a strongly-typed payload.
    /// </summary>
    /// <typeparam name="TPayload">The payload type implementing <see cref="IEventPayload"/>.</typeparam>
    /// <param name="time">The absolute simulation time for the event.</param>
    /// <param name="type">The event type identifier.</param>
    /// <param name="payload">The strongly-typed payload.</param>
    /// <param name="priority">The priority for ordering events at the same time (default 0).</param>
    /// <param name="handler">Optional handler override; uses registered handler if null.</param>
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

    /// <summary>
    /// Schedules an event relative to the current time.
    /// </summary>
    /// <param name="dt">The time delta from now when the event should occur.</param>
    /// <param name="type">The event type identifier.</param>
    /// <param name="payload">Optional dictionary payload for event data.</param>
    /// <param name="priority">The priority for ordering events at the same time (default 0).</param>
    /// <param name="handler">Optional handler override; uses registered handler if null.</param>
    public void ScheduleIn(
        double dt,
        string type,
        IDictionary<string, object?>? payload = null,
        int priority = 0,
        EventHandlerDelegate? handler = null)
    {
        Schedule(Time + dt, type, payload, priority, handler);
    }

    /// <summary>
    /// Schedules an event relative to the current time with a strongly-typed payload.
    /// </summary>
    /// <typeparam name="TPayload">The payload type implementing <see cref="IEventPayload"/>.</typeparam>
    /// <param name="dt">The time delta from now when the event should occur.</param>
    /// <param name="type">The event type identifier.</param>
    /// <param name="payload">The strongly-typed payload.</param>
    /// <param name="priority">The priority for ordering events at the same time (default 0).</param>
    /// <param name="handler">Optional handler override; uses registered handler if null.</param>
    public void ScheduleIn<TPayload>(
        double dt,
        string type,
        TPayload payload,
        int priority = 0,
        EventHandlerDelegate? handler = null) where TPayload : IEventPayload
    {
        Schedule(Time + dt, type, payload, priority, handler);
    }

    /// <summary>
    /// Stops the simulation loop.
    /// </summary>
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
