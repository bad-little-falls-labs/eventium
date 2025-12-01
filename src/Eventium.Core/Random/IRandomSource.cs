// <copyright file="IRandomSource.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.Random;

/// <summary>
/// Abstraction for pseudo-random number generation.
/// </summary>
public interface IRandomSource
{
    double NextDouble();
    int NextInt(int minInclusive, int maxExclusive);
}
