namespace Eventium.Core.Events;

/// <summary>
/// Delegate for handling simulation events.
/// </summary>
/// <param name="context">The simulation context providing access to world, time, scheduling, etc.</param>
/// <param name="evt">The event being handled.</param>
public delegate void EventHandlerDelegate(ISimulationContext context, Event evt);
