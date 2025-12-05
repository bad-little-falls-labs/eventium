// <copyright file="EmptyPayload.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.Events;

/// <summary>
/// A payload type for events that carry no data.
/// </summary>
public sealed record EmptyPayload : IEventPayload
{
    /// <summary>
    /// Gets a reusable singleton instance of <see cref="EmptyPayload"/>.
    /// </summary>
    public static readonly EmptyPayload Instance = new();

    private EmptyPayload() { }
}
