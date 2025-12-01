// <copyright file="ISystem.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
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
    /// <param name="context">The simulation context.</param>
    /// <param name="evt">The event to handle.</param>
    void HandleEvent(ISimulationContext context, Event evt);
}
