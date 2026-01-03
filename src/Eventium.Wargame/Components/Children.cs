// <copyright file="Children.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core.World;

namespace Eventium.Wargame.Components;

/// <summary>
/// Represents child entities in a hierarchical formation structure.
/// </summary>
public sealed class Children : IComponent
{
    /// <summary>Gets or sets the list of child entity IDs.</summary>
    public List<int> ChildEntityIds { get; set; } = new();

    /// <summary>Gets a value indicating whether this unit has any children.</summary>
    public bool HasChildren => ChildEntityIds.Count > 0;
}
