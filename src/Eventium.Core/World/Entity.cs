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

    public Entity(int id, string type)
    {
        Id = id;
        Type = type;
    }

    public int Id { get; }
    public string Type { get; }

    public void AddComponent(string name, IComponent component)
    {
        _components[name] = component;
    }

    public T? GetComponent<T>(string name) where T : class, IComponent
    {
        return _components.TryGetValue(name, out var comp) ? comp as T : null;
    }
}
