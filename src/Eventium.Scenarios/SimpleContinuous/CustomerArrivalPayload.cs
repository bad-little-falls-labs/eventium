// <copyright file="CustomerArrivalPayload.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.Events;

namespace Eventium.Scenarios.SimpleContinuous;

/// <summary>
/// Payload for the CustomerArrival event.
/// Carries no additional data - uses EmptyPayload pattern.
/// </summary>
public sealed record CustomerArrivalPayload : IEventPayload
{
    public static readonly CustomerArrivalPayload Instance = new();

    private CustomerArrivalPayload() { }
}
