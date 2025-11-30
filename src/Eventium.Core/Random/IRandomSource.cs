namespace Eventium.Core.Random;

/// <summary>
/// Abstraction for pseudo-random number generation.
/// </summary>
public interface IRandomSource
{
    double NextDouble();
    int NextInt(int minInclusive, int maxExclusive);
}
