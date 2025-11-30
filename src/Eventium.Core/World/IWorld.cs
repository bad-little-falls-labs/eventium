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
    void AddEntity(Entity entity);

    /// <summary>
    /// Gets an entity by ID, or null if not found.
    /// </summary>
    Entity? GetEntity(int id);
}
