// <copyright file="ArrivalComponent.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.World;

namespace Eventium.Scenarios.SimpleContinuous;

public sealed class ArrivalComponent : IComponent
{
    public double ArrivedAt { get; set; }
}
