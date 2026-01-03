// <copyright file="MoraleComponent.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Wargame.Components;

/// <summary>
/// Represents the morale state of a unit. Low morale can lead to routing.
/// </summary>
public sealed class MoraleComponent
{
    /// <summary>Gets or sets the maximum morale.</summary>
    public int MaxMorale { get; set; }

    /// <summary>Gets or sets the current morale.</summary>
    public int CurrentMorale { get; set; }

    /// <summary>Gets or sets a value indicating whether the unit is routed.</summary>
    public bool IsRouted { get; set; }
}
