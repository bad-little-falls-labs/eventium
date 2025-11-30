using Eventium.Core.Events;
using Eventium.Core.Systems;

namespace Eventium.Core;

/// <summary>
/// Abstraction for the central simulation orchestrator.
/// Extends ISimulationContext with engine-specific operations.
/// </summary>
public interface ISimulationEngine : ISimulationContext
{
    /// <summary>
    /// Gets the event queue.
    /// </summary>
    IEventQueue Queue { get; }

    /// <summary>
    /// Registers a handler for a specific event type.
    /// </summary>
    void RegisterHandler(string eventType, EventHandlerDelegate handler);

    /// <summary>
    /// Registers a system to handle events.
    /// </summary>
    void RegisterSystem(ISystem system);

    /// <summary>
    /// Runs the simulation until a condition is met.
    /// </summary>
    /// <param name="untilTime">Optional maximum simulation time.</param>
    /// <param name="maxEvents">Optional maximum number of events to process.</param>
    /// <returns>A result containing statistics about the simulation run.</returns>
    SimulationResult Run(double? untilTime = null, int? maxEvents = null);
}
