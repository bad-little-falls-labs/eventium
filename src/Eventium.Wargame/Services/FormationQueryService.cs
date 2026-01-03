// <copyright file="FormationQueryService.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core;
using Eventium.Core.World;
using Eventium.Wargame.Components;
using Eventium.Wargame.Domain;

namespace Eventium.Wargame.Services;

/// <summary>
/// Service for querying and traversing the formation hierarchy.
/// </summary>
public sealed class FormationQueryService
{
    private readonly IWorld _world;

    /// <summary>
    /// Initializes a new instance of the FormationQueryService.
    /// </summary>
    public FormationQueryService(IWorld world)
    {
        _world = world;
    }

    /// <summary>
    /// Gets all descendants of a formation (children, grandchildren, etc.) recursively.
    /// </summary>
    public IEnumerable<Entity> GetAllDescendants(int parentEntityId)
    {
        var descendants = new List<Entity>();
        var queue = new Queue<int>();
        queue.Enqueue(parentEntityId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();
            var children = GetChildren(currentId);

            foreach (var child in children)
            {
                descendants.Add(child);
                queue.Enqueue(child.Id);
            }
        }

        return descendants;
    }

    /// <summary>
    /// Gets all ancestors of a unit (parents, grandparents, etc.).
    /// </summary>
    public IEnumerable<Entity> GetAncestors(int entityId)
    {
        var ancestors = new List<Entity>();
        var current = GetParent(entityId);

        while (current != null)
        {
            ancestors.Add(current);
            current = GetParent(current.Id);
        }

        return ancestors;
    }

    /// <summary>
    /// Gets all immediate children of a formation entity.
    /// </summary>
    public IEnumerable<Entity> GetChildren(int parentEntityId)
    {
        var parent = _world.GetEntity(parentEntityId);
        if (parent == null) return Enumerable.Empty<Entity>();

        var childrenComp = parent.GetComponent<Children>("CHILDREN");
        if (childrenComp == null) return Enumerable.Empty<Entity>();

        return childrenComp.ChildEntityIds
            .Select(childId => _world.GetEntity(childId))
            .Where(e => e != null)!;
    }

    /// <summary>
    /// Gets the parent of a unit entity.
    /// </summary>
    public Entity? GetParent(int entityId)
    {
        var entity = _world.GetEntity(entityId);
        if (entity == null) return null;

        var parentComp = entity.GetComponent<Parent>("PARENT");
        if (parentComp == null || !parentComp.IsActive) return null;

        return _world.GetEntity(parentComp.ParentEntityId);
    }

    /// <summary>
    /// Gets the total strength of a formation including all descendants.
    /// </summary>
    public int GetTotalStrength(int formationEntityId)
    {
        var formation = _world.GetEntity(formationEntityId);
        if (formation == null) return 0;

        var health = formation.GetComponent<Health>("HEALTH");
        int strength = health?.CurrentStrength ?? 0;

        foreach (var descendant in GetAllDescendants(formationEntityId))
        {
            var descendantHealth = descendant.GetComponent<Health>("HEALTH");
            if (descendantHealth != null)
            {
                strength += descendantHealth.CurrentStrength;
            }
        }

        return strength;
    }

    /// <summary>
    /// Gets all units at a specific echelon level.
    /// </summary>
    public IEnumerable<Entity> GetUnitsAtEchelon(EchelonLevel level)
    {
        return _world.Entities.Values.Where(e =>
        {
            var echelon = e.GetComponent<Echelon>("ECHELON");
            return echelon != null && echelon.Level == level;
        });
    }

    /// <summary>
    /// Gets all units belonging to a specific faction.
    /// </summary>
    public IEnumerable<Entity> GetUnitsByFaction(string faction)
    {
        return _world.Entities.Values.Where(e =>
        {
            var echelon = e.GetComponent<Echelon>("ECHELON");
            return echelon != null && echelon.Faction == faction;
        });
    }

    /// <summary>
    /// Validates whether a formation hierarchy is coherent (no cycles, all references valid).
    /// </summary>
    public bool IsHierarchyCoherent(int formationEntityId)
    {
        var visited = new HashSet<int>();
        return !HasCycle(formationEntityId, visited);
    }

    /// <summary>
    /// Checks for cycles in the hierarchy.
    /// </summary>
    private bool HasCycle(int entityId, HashSet<int> visited)
    {
        if (visited.Contains(entityId)) return true;
        visited.Add(entityId);

        var children = GetChildren(entityId);
        foreach (var child in children)
        {
            if (HasCycle(child.Id, new HashSet<int>(visited)))
            {
                return true;
            }
        }

        return false;
    }
}
