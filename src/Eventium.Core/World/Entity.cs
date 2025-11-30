using System.Collections.Generic;

namespace Eventium.Core.World;

/// <summary>
/// A world entity composed of named components.
/// </summary>
public sealed class Entity
{
    private readonly Dictionary<string, IComponent> _components = new();

    public int Id { get; }
    public string Type { get; }

    public Entity(int id, string type)
    {
        Id = id;
        Type = type;
    }

    public void AddComponent(string name, IComponent component)
    {
        _components[name] = component;
    }

    public T? GetComponent<T>(string name) where T : class, IComponent
    {
        return _components.TryGetValue(name, out var comp) ? comp as T : null;
    }
}
