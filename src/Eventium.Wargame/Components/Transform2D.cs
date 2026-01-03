// <copyright file="Transform2D.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core.World;

namespace Eventium.Wargame.Components;

/// <summary>
/// Represents a 2D transformation (position and rotation) for a unit or formation.
/// </summary>
public sealed class Transform2D : IComponent
{

    /// <summary>Gets or sets the rotation angle in degrees (0-360).</summary>
    public double RotationDegrees { get; set; }

    /// <summary>Gets or sets the scale factor (1.0 = full size).</summary>
    public double Scale { get; set; } = 1.0;
    /// <summary>Gets or sets the X coordinate on the battlefield.</summary>
    public double X { get; set; }

    /// <summary>Gets or sets the Y coordinate on the battlefield.</summary>
    public double Y { get; set; }
}
