// <copyright file="ArrivalComponent.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.World;
using MemoryPack;

namespace Eventium.Scenarios.SimpleContinuous;

[MemoryPackable]
public sealed partial class ArrivalComponent : IComponent
{
    public double ArrivedAt { get; set; }
}
