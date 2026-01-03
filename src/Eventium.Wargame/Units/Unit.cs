// <copyright file="Unit.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Wargame.Units;

/// <summary>
/// Represents a military unit in the wargame with health, morale, and combat stats.
/// </summary>
public sealed record Unit(
    int Id,
    string Name,
    string Faction,
    int MaxHealth,
    int CurrentHealth,
    int Morale,
    int Combat,
    int X,
    int Y);
