// <copyright file="Parent.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core.World;

namespace Eventium.Wargame.Components;

/// <summary>
/// Represents the parent entity in a hierarchical formation structure.
/// </summary>
public sealed class Parent : IComponent
{

    /// <summary>Gets or sets a value indicating whether the parent relationship is active.</summary>
    public bool IsActive { get; set; } = true;
    /// <summary>Gets or sets the entity ID of the parent unit.</summary>
    public int ParentEntityId { get; set; }
}
