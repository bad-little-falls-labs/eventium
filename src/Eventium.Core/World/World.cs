using System.Collections.Generic;

// <copyright file="World.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.World;

/// <summary>
/// The simulated world containing entities and global state.
/// </summary>
public sealed class World : IWorld
{
    private readonly Dictionary<int, Entity> _entities = new();
    private readonly Dictionary<string, object?> _globals = new();

    /// <summary>
    /// Gets all entities in the world as a read-only dictionary.
    /// </summary>
    public IReadOnlyDictionary<int, Entity> Entities => _entities;

    /// <summary>
    /// Gets the global state dictionary for arbitrary scenario and system data.
    /// </summary>
    public IDictionary<string, object?> Globals => _globals;

    /// <inheritdoc />
    public void AddEntity(Entity entity)
    {
        _entities[entity.Id] = entity;
    }

    /// <inheritdoc />
    public Entity? GetEntity(int id)
    {
        return _entities.TryGetValue(id, out var entity) ? entity : null;
    }
}
