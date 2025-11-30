using System.Collections.Generic;

namespace Eventium.Core.World;

/// <summary>
/// The simulated world containing entities and global state.
/// </summary>
public sealed class World
{
    private readonly Dictionary<int, Entity> _entities = new();

    public IReadOnlyDictionary<int, Entity> Entities => _entities;

    /// <summary>
    /// Arbitrary global state for scenarios and systems.
    /// </summary>
    public Dictionary<string, object?> Globals { get; } = new();

    public void AddEntity(Entity entity)
    {
        _entities[entity.Id] = entity;
    }

    public Entity? GetEntity(int id)
    {
        return _entities.TryGetValue(id, out var entity) ? entity : null;
    }
}
