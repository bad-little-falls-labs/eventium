// <copyright file="DefaultRandomSource.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.Random;

/// <summary>
/// Default RNG implementation using System.Random.
/// </summary>
public sealed class DefaultRandomSource : IRandomSource
{
    private readonly System.Random _random;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultRandomSource"/> class.
    /// </summary>
    /// <param name="seed">Optional seed for reproducible random sequences. If null, uses time-based seed.</param>
    public DefaultRandomSource(int? seed = null)
    {
        _random = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
    }

    /// <inheritdoc />
    public double NextDouble() => _random.NextDouble();

    /// <inheritdoc />
    public int NextInt(int minInclusive, int maxExclusive) =>
        _random.Next(minInclusive, maxExclusive);
}
