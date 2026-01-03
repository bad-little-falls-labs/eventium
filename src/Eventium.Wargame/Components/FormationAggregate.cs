// <copyright file="FormationAggregate.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core.World;

namespace Eventium.Wargame.Components;

/// <summary>
/// Represents aggregate metrics of a formation, computed from child units.
/// </summary>
public sealed class FormationAggregate : IComponent
{

    /// <summary>Gets or sets the total strength aggregated from all children.</summary>
    public int AggregatedStrength { get; set; }

    /// <summary>Gets or sets the count of immediate children.</summary>
    public int ImmediateChildCount { get; set; }

    /// <summary>Gets or sets a value indicating whether aggregation is dirty (needs update).</summary>
    public bool IsDirty { get; set; } = true;
    /// <summary>Gets or sets the timestamp of the last aggregation pulse (turn number).</summary>
    public double LastAggregationTime { get; set; }

    /// <summary>Gets or sets the total recursive child count (all descendants).</summary>
    public int TotalDescendantCount { get; set; }
}
