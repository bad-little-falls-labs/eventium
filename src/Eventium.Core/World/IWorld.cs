// <copyright file="IWorld.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.World;

/// <summary>
/// Abstraction for the simulated world containing entities and global state.
/// </summary>
public interface IWorld
{
    /// <summary>
    /// Gets all entities in the world.
    /// </summary>
    IReadOnlyDictionary<int, Entity> Entities { get; }

    /// <summary>
    /// Gets the global state dictionary.
    /// </summary>
    IDictionary<string, object?> Globals { get; }

    /// <summary>
    /// Adds an entity to the world.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void AddEntity(Entity entity);

    /// <summary>
    /// Gets an entity by ID, or null if not found.
    /// </summary>
    /// <param name="id">The unique entity ID.</param>
    /// <returns>The entity if found, otherwise null.</returns>
    Entity? GetEntity(int id);
}
