// <copyright file="Health.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core.World;

namespace Eventium.Wargame.Components;

/// <summary>
/// Represents the health state of a unit or formation.
/// </summary>
public sealed class Health : IComponent
{

    /// <summary>Gets or sets the armor value (damage reduction factor, 0-100).</summary>
    public int Armor { get; set; }

    /// <summary>Gets or sets the current health/personnel strength.</summary>
    public int CurrentStrength { get; set; }

    /// <summary>Gets or sets a value indicating whether the unit is destroyed.</summary>
    public bool IsDestroyed => CurrentStrength <= 0;
    /// <summary>Gets or sets the maximum health/personnel strength.</summary>
    public int MaxStrength { get; set; }
}
