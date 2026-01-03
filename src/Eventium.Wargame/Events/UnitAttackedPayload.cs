// <copyright file="UnitAttackedPayload.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core.Events;

namespace Eventium.Wargame.Events;

/// <summary>
/// Payload for when a unit attacks another unit.
/// </summary>
public sealed record UnitAttackedPayload(
    int AttackerUnitId,
    int DefenderUnitId,
    int Damage,
    bool Hit) : IEventPayload;
