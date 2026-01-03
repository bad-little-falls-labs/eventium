// <copyright file="PositionComponent.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Wargame.Components;

/// <summary>
/// Represents the position of a unit on the battlefield grid.
/// </summary>
public sealed class PositionComponent
{
    /// <summary>Gets or sets the X coordinate.</summary>
    public int X { get; set; }

    /// <summary>Gets or sets the Y coordinate.</summary>
    public int Y { get; set; }
}
