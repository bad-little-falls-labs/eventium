// <copyright file="HierarchyValidationSystem.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core;
using Eventium.Core.Systems;
using Eventium.Wargame.Components;

namespace Eventium.Wargame.Systems;

/// <summary>
/// System that validates the integrity of the formation hierarchy.
/// Ensures parent-child relationships are consistent and valid.
/// </summary>
public sealed class HierarchyValidationSystem : ISystem
{
    /// <summary>
    /// Handles hierarchy validation events.
    /// </summary>
    public IEnumerable<string> HandledEventTypes => new[] { "VALIDATE_HIERARCHY" };

    /// <summary>
    /// Validates the hierarchy structure and reports any inconsistencies.
    /// </summary>
    public void HandleEvent(ISimulationContext context, Core.Events.Event evt)
    {
        ValidateAllHierarchies(context);
    }

    /// <summary>
    /// Validates all parent-child relationships in the world.
    /// </summary>
    private static void ValidateAllHierarchies(ISimulationContext context)
    {
        var allEntities = context.World.Entities.Values.ToList();

        foreach (var entity in allEntities)
        {
            ValidateEntity(entity, context, allEntities);
        }
    }

    /// <summary>
    /// Validates a single entity's parent-child relationships.
    /// </summary>
    private static void ValidateEntity(Core.World.Entity entity, ISimulationContext context, List<Core.World.Entity> allEntities)
    {
        var parent = entity.GetComponent<Parent>("PARENT");
        var children = entity.GetComponent<Children>("CHILDREN");

        // Validate parent reference if it exists
        if (parent != null && parent.IsActive)
        {
            var parentEntity = allEntities.FirstOrDefault(e => e.Id == parent.ParentEntityId);
            if (parentEntity == null)
            {
                // Parent doesn't exist - mark as inactive
                parent.IsActive = false;
            }
            else
            {
                // Verify parent's children list includes this entity
                var parentChildren = parentEntity.GetComponent<Children>("CHILDREN");
                if (parentChildren != null && !parentChildren.ChildEntityIds.Contains(entity.Id))
                {
                    parentChildren.ChildEntityIds.Add(entity.Id);
                }
            }
        }

        // Validate children references if they exist
        if (children != null)
        {
            var invalidChildren = new List<int>();
            foreach (var childId in children.ChildEntityIds)
            {
                var childEntity = allEntities.FirstOrDefault(e => e.Id == childId);
                if (childEntity == null)
                {
                    invalidChildren.Add(childId);
                }
                else
                {
                    // Verify child's parent reference points to this entity
                    var childParent = childEntity.GetComponent<Parent>("PARENT");
                    if (childParent == null || childParent.ParentEntityId != entity.Id)
                    {
                        if (childParent == null)
                        {
                            childParent = new Parent { ParentEntityId = entity.Id, IsActive = true };
                            childEntity.AddComponent("PARENT", childParent);
                        }
                        else
                        {
                            childParent.ParentEntityId = entity.Id;
                        }
                    }
                }
            }

            // Remove invalid children references
            foreach (var invalidChildId in invalidChildren)
            {
                children.ChildEntityIds.Remove(invalidChildId);
            }
        }
    }
}
