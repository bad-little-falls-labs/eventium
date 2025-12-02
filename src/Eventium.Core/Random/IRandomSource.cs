// <copyright file="IRandomSource.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.Random;

/// <summary>
/// Abstraction for pseudo-random number generation.
/// </summary>
public interface IRandomSource
{
    /// <summary>
    /// Returns a random double in the range [0.0, 1.0).
    /// </summary>
    /// <returns>A random value between 0.0 (inclusive) and 1.0 (exclusive).</returns>
    double NextDouble();

    /// <summary>
    /// Returns a random integer in the specified range.
    /// </summary>
    /// <param name="minInclusive">The inclusive lower bound.</param>
    /// <param name="maxExclusive">The exclusive upper bound.</param>
    /// <returns>A random integer in [minInclusive, maxExclusive).</returns>
    int NextInt(int minInclusive, int maxExclusive);
}
