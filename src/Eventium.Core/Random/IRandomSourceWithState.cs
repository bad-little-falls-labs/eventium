// <copyright file="IRandomSourceWithState.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Core.Random;

/// <summary>
/// Extends IRandomSource with state capture and restoration capabilities.
/// Enables RNG state snapshots for simulation rollback and reproducibility.
/// </summary>
public interface IRandomSourceWithState : IRandomSource
{
    /// <summary>
    /// Captures the current internal state of the RNG.
    /// </summary>
    /// <returns>An object representing the current RNG state. Can be restored via SetState().</returns>
    object GetState();

    /// <summary>
    /// Restores the RNG to a previously captured state.
    /// </summary>
    /// <param name="state">The state object returned by a prior GetState() call.</param>
    /// <exception cref="ArgumentException">Thrown if state is invalid or from a different RNG implementation.</exception>
    void SetState(object state);
}
