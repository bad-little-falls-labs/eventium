// <copyright file="ISimulationContext.cs" company="bad-little-falls-labs">
//  Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.Events;
using Eventium.Core.Instrumentation;
using Eventium.Core.Random;
using Eventium.Core.Time;
using Eventium.Core.World;

namespace Eventium.Core;

/// <summary>
/// A narrower view of the simulation engine for use by systems and event handlers.
/// Provides only what systems need to do their job.
/// </summary>
public interface ISimulationContext
{

    /// <summary>
    /// Gets the metrics registry.
    /// </summary>
    MetricsRegistry Metrics { get; }

    /// <summary>
    /// Gets the random number generator.
    /// </summary>
    IRandomSource Rng { get; }
    /// <summary>
    /// Gets the current simulation time.
    /// </summary>
    double Time { get; }

    /// <summary>
    /// Gets the time model (discrete or continuous).
    /// </summary>
    TimeModel TimeModel { get; }

    /// <summary>
    /// Gets the simulated world.
    /// </summary>
    IWorld World { get; }

    /// <summary>
    /// Schedules an event at an absolute time.
    /// </summary>
    void Schedule(
        double time,
        string type,
        IDictionary<string, object?>? payload = null,
        int priority = 0,
        EventHandlerDelegate? handler = null);

    /// <summary>
    /// Schedules an event at an absolute time with a strongly-typed payload.
    /// </summary>
    void Schedule<TPayload>(
        double time,
        string type,
        TPayload payload,
        int priority = 0,
        EventHandlerDelegate? handler = null) where TPayload : IEventPayload;

    /// <summary>
    /// Schedules an event relative to the current time.
    /// </summary>
    void ScheduleIn(
        double dt,
        string type,
        IDictionary<string, object?>? payload = null,
        int priority = 0,
        EventHandlerDelegate? handler = null);

    /// <summary>
    /// Schedules an event relative to the current time with a strongly-typed payload.
    /// </summary>
    void ScheduleIn<TPayload>(
        double dt,
        string type,
        TPayload payload,
        int priority = 0,
        EventHandlerDelegate? handler = null) where TPayload : IEventPayload;

    /// <summary>
    /// Stops the simulation loop.
    /// </summary>
    void Stop();
}
