// <copyright file="StepPolicy.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Core.Time;

/// <summary>
/// Defines how a simulation advances through steps when using a time controller.
/// </summary>
public enum StepPolicy
{
    /// <summary>
    /// Advance by processing the next scheduled event.
    /// </summary>
    Event,

    /// <summary>
    /// Advance by a fixed time increment (TimeModel.Step for discrete, or configurable for continuous).
    /// </summary>
    Tick,

    /// <summary>
    /// Advance to the next logical turn/phase boundary (primarily for discrete simulations).
    /// </summary>
    /// <remarks>
    /// A turn advance is defined as advancing the boundary by <c>TimeModel.Step</c> (or configured turn length) from the current boundary,
    /// then processing all events with <c>evt.Time &lt;= nextBoundary</c>. This makes end-of-turn events fire reliably at the boundary.
    /// </remarks>
    Turn
}
