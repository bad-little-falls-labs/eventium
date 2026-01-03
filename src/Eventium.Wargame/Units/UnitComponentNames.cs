// <copyright file="UnitComponentNames.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Wargame.Units;

/// <summary>
/// Defines component names for unit entities in the world.
/// </summary>
public static class UnitComponentNames
{
    /// <summary>Component name for unit position data (X, Y).</summary>
    public const string Position = "UNIT_POSITION";

    /// <summary>Component name for unit health and armor data.</summary>
    public const string Health = "UNIT_HEALTH";

    /// <summary>Component name for unit morale and routing status.</summary>
    public const string Morale = "UNIT_MORALE";

    /// <summary>Component name for unit combat capabilities.</summary>
    public const string Combat = "UNIT_COMBAT";

    /// <summary>Component name for unit faction affiliation.</summary>
    public const string Faction = "UNIT_FACTION";
}
