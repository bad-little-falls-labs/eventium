// <copyright file="HealthComponent.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Wargame.Components;

/// <summary>
/// Represents the health and damage state of a unit.
/// </summary>
public sealed class HealthComponent
{
    /// <summary>Gets or sets the maximum health.</summary>
    public int MaxHealth { get; set; }

    /// <summary>Gets or sets the current health.</summary>
    public int CurrentHealth { get; set; }

    /// <summary>Gets or sets the armor value (damage reduction).</summary>
    public int Armor { get; set; }
}
