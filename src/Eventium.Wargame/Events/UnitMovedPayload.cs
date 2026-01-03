// <copyright file="UnitMovedPayload.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core.Events;

namespace Eventium.Wargame.Events;

/// <summary>
/// Payload for when a unit moves to a new position.
/// </summary>
public sealed record UnitMovedPayload(
    int UnitId,
    int FromX,
    int FromY,
    int ToX,
    int ToY) : IEventPayload;
