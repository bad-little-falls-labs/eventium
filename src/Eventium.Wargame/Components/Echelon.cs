// <copyright file="Echelon.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core.World;
using Eventium.Wargame.Domain;

namespace Eventium.Wargame.Components;

/// <summary>
/// Represents the echelon (hierarchical level) of a military unit or formation.
/// </summary>
public sealed class Echelon : IComponent
{

    /// <summary>Gets or sets the unit identifier (e.g., "1st Battalion", "A Squad").</summary>
    public string Designation { get; set; } = string.Empty;

    /// <summary>Gets or sets the faction/side the unit belongs to.</summary>
    public string Faction { get; set; } = string.Empty;
    /// <summary>Gets or sets the echelon level.</summary>
    public EchelonLevel Level { get; set; }
}
