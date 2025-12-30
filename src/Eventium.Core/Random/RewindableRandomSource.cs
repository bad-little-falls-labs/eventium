// <copyright file="RewindableRandomSource.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Core.Random;

/// <summary>
/// A rewindable pseudo-random number generator with full state capture support.
/// Implements the PCG (Permuted Congruential Generator) algorithm for deterministic,
/// reproducible, and stateful random number generation.
/// </summary>
public sealed class RewindableRandomSource : IRandomSourceWithState
{
    private const ulong DefaultIncrement = 1442695040888963407UL;
    private ulong _state;
    private readonly ulong _increment;

    /// <summary>
    /// Initializes a new instance of the <see cref="RewindableRandomSource"/> class.
    /// </summary>
    /// <param name="seed">Optional seed for the RNG. If null, uses a time-based seed.</param>
    public RewindableRandomSource(int? seed = null)
    {
        _increment = DefaultIncrement;
        
        // Initialize state from seed
        var initialSeed = seed ?? (int)System.DateTime.UtcNow.Ticks;
        _state = ((ulong)initialSeed ^ DefaultIncrement) * 6364136223846793005UL + DefaultIncrement;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RewindableRandomSource"/> class with a custom increment.
    /// </summary>
    /// <param name="seed">Optional seed for the RNG.</param>
    /// <param name="increment">Custom increment for the PCG algorithm.</param>
    internal RewindableRandomSource(int? seed, ulong increment)
    {
        _increment = increment;
        var initialSeed = seed ?? (int)System.DateTime.UtcNow.Ticks;
        _state = ((ulong)initialSeed ^ increment) * 6364136223846793005UL + increment;
    }

    /// <inheritdoc />
    public double NextDouble()
    {
        var value = NextUInt();
        return value * (1.0 / 4294967296.0);
    }

    /// <inheritdoc />
    public int NextInt(int minInclusive, int maxExclusive)
    {
        if (minInclusive >= maxExclusive)
        {
            throw new ArgumentException($"{nameof(minInclusive)} must be less than {nameof(maxExclusive)}.");
        }

        var range = (ulong)(maxExclusive - minInclusive);
        var sample = NextUInt() % range;
        return minInclusive + (int)sample;
    }

    /// <inheritdoc />
    public object GetState()
    {
        return new RngStateSnapshot(_state, _increment);
    }

    /// <inheritdoc />
    public void SetState(object state)
    {
        if (state is not RngStateSnapshot snapshot)
        {
            throw new ArgumentException(
                "State must be a RngStateSnapshot from the same RewindableRandomSource.",
                nameof(state));
        }

        if (snapshot.Increment != _increment)
        {
            throw new ArgumentException(
                "Cannot restore state: increment mismatch. State is from a different RNG instance.",
                nameof(state));
        }

        _state = snapshot.State;
    }

    private uint NextUInt()
    {
        var oldState = _state;
        _state = oldState * 6364136223846793005UL + _increment;

        // XorShift + rotation (permutation)
        uint xorshifted = (uint)(((oldState >> 18) ^ oldState) >> 27);
        int rot = (int)(oldState >> 59);
        return (xorshifted >> rot) | (xorshifted << (-rot & 31));
    }

    /// <summary>
    /// Immutable snapshot of RNG state for restoration.
    /// </summary>
    private sealed class RngStateSnapshot
    {
        public RngStateSnapshot(ulong state, ulong increment)
        {
            State = state;
            Increment = increment;
        }

        public ulong State { get; }
        public ulong Increment { get; }
    }
}
