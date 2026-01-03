// <copyright file="AggregationPulse.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core.Events;

namespace Eventium.Wargame.Events;

/// <summary>
/// Payload for an aggregation pulse event that triggers formation metric recalculation.
/// </summary>
public sealed record AggregationPulse(
    int FormationEntityId,
    double PulseTime) : IEventPayload;
