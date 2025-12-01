// <copyright file="MoveAgentPayload.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.Events;

namespace Eventium.Scenarios.SimpleDiscrete;

/// <summary>
/// Payload for the MoveAgent event.
/// </summary>
public sealed record MoveAgentPayload(int EntityId, int Dx, int Dy) : IEventPayload;
