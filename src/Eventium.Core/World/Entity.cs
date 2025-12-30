// <copyright file="Entity.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using System.Collections.Generic;

namespace Eventium.Core.World;

/// <summary>
/// A world entity composed of named components.
/// </summary>
public sealed class Entity
{
    private readonly Dictionary<string, IComponent> _components = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    /// <param name="id">The unique entity identifier.</param>
    /// <param name="type">The entity type (e.g., "agent", "resource").</param>
    public Entity(int id, string type)
    {
        Id = id;
        Type = type;
    }

    /// <summary>
    /// Gets the components attached to this entity.
    /// </summary>
    public IReadOnlyDictionary<string, IComponent> Components => _components;

    /// <summary>
    /// Gets the unique identifier for this entity.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the entity type.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Adds or updates a component on this entity.
    /// </summary>
    /// <param name="name">The component name/key.</param>
    /// <param name="component">The component instance.</param>
    public void AddComponent(string name, IComponent component)
    {
        _components[name] = component;
    }

    /// <summary>
    /// Gets a component by name and type.
    /// </summary>
    /// <typeparam name="T">The component type.</typeparam>
    /// <param name="name">The component name/key.</param>
    /// <returns>The component if found and of the correct type, otherwise null.</returns>
    public T? GetComponent<T>(string name) where T : class, IComponent
    {
        return _components.TryGetValue(name, out var comp) ? comp as T : null;
    }
}
