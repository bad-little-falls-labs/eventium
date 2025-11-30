using System.Collections.Generic;
using Eventium.Core.Events;

namespace Eventium.Core.Systems;

/// <summary>
/// A system implements domain logic by handling events.
/// </summary>
public interface ISystem
{
    /// <summary>
    /// The event types this system is interested in.
    /// </summary>
    IEnumerable<string> HandledEventTypes { get; }

    /// <summary>
    /// Handles a dispatched event.
    /// </summary>
    void HandleEvent(SimulationEngine engine, Event evt);
}
