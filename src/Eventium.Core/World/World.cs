using System;
using System.Collections.Generic;
using System.Linq;
using Eventium.Core.Snapshots;
using MemoryPack;

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
    public WorldSnapshot CaptureSnapshot()
    {
        var entities = new List<WorldSnapshot.EntitySnapshot>(_entities.Count);

        foreach (var entity in _entities.Values.OrderBy(e => e.Id))
        {
            var clonedComponents = new Dictionary<string, IComponent>(entity.Components.Count);
            foreach (var component in entity.Components.OrderBy(comp => comp.Key))
            {
                clonedComponents[component.Key] = CloneComponent(component.Value);
            }

            entities.Add(new WorldSnapshot.EntitySnapshot(
                entity.Id,
                entity.Type,
                clonedComponents.Count,
                clonedComponents));
        }

        var globalsCopy = new Dictionary<string, object?>(_globals.Count);
        foreach (var kvp in _globals.OrderBy(kvp => kvp.Key))
        {
            globalsCopy[kvp.Key] = CloneObject(kvp.Value);
        }

        return new WorldSnapshot(entities.Count, entities, globalsCopy);
    }

    /// <inheritdoc />
    public Entity? GetEntity(int id)
    {
        return _entities.TryGetValue(id, out var entity) ? entity : null;
    }

    /// <inheritdoc />
    public void RestoreSnapshot(WorldSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        _entities.Clear();
        _globals.Clear();

        foreach (var global in snapshot.Globals)
        {
            _globals[global.Key] = CloneObject(global.Value);
        }

        foreach (var entitySnapshot in snapshot.Entities)
        {
            var entity = new Entity(entitySnapshot.Id, entitySnapshot.Type);
            foreach (var component in entitySnapshot.Components)
            {
                entity.AddComponent(component.Key, CloneComponent(component.Value));
            }

            _entities[entity.Id] = entity;
        }
    }

    private static IComponent CloneComponent(IComponent component)
    {
        ArgumentNullException.ThrowIfNull(component);
        var type = component.GetType();
        var bytes = MemoryPackSerializer.Serialize(type, component);
        return (IComponent)MemoryPackSerializer.Deserialize(type, bytes)!;
    }

    private static object? CloneObject(object? value)
    {
        if (value is null)
        {
            return null;
        }

        var type = value.GetType();
        var bytes = MemoryPackSerializer.Serialize(type, value);
        return MemoryPackSerializer.Deserialize(type, bytes);
    }
}
