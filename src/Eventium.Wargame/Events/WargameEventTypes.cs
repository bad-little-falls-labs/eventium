// <copyright file="WargameEventTypes.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Wargame.Events;

/// <summary>
/// Defines standard event types for wargame simulations.
/// </summary>
public static class WargameEventTypes
{
    /// <summary>Event fired when a unit moves to a new position.</summary>
    public const string UnitMoved = "UNIT_MOVED";

    /// <summary>Event fired when a unit attacks another unit.</summary>
    public const string UnitAttacked = "UNIT_ATTACKED";

    /// <summary>Event fired when a unit takes damage.</summary>
    public const string UnitDamaged = "UNIT_DAMAGED";

    /// <summary>Event fired when a unit is routed (loses morale and flees).</summary>
    public const string UnitRouted = "UNIT_ROUTED";

    /// <summary>Event fired when a unit is destroyed/eliminated.</summary>
    public const string UnitDestroyed = "UNIT_DESTROYED";

    /// <summary>Event fired at the start of each turn for a faction.</summary>
    public const string FactionTurnStart = "FACTION_TURN_START";

    /// <summary>Event fired at the end of each turn for a faction.</summary>
    public const string FactionTurnEnd = "FACTION_TURN_END";
}
