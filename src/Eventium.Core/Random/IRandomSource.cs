// <copyright file="IRandomSource.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.Random;

/// <summary>
/// Abstraction over pseudo-random number generation used by simulations.
/// Implementations should provide deterministic sequences when seeded
/// to enable reproducible simulation runs.
/// </summary>
public interface IRandomSource
{
    /// <summary>
    /// Returns the next double in the range [0.0, 1.0).
    /// </summary>
    /// <returns>A random value in [0.0, 1.0).</returns>
    double NextDouble();

    /// <summary>
    /// Returns the next integer within the specified range.
    /// </summary>
    /// <param name="minInclusive">The inclusive lower bound.</param>
    /// <param name="maxExclusive">The exclusive upper bound.</param>
    /// <returns>An integer in [minInclusive, maxExclusive).</returns>
    int NextInt(int minInclusive, int maxExclusive);
}
