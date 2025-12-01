// <copyright file="PositionComponent.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.World;

namespace Eventium.Scenarios.SimpleDiscrete;

public sealed class PositionComponent : IComponent
{
    public int X { get; set; }
    public int Y { get; set; }
}
