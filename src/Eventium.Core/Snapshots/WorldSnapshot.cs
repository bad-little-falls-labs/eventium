// <copyright file="WorldSnapshot.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using System.Collections.Generic;
using Eventium.Core.World;

namespace Eventium.Core.Snapshots;

/// <summary>
/// Captures the state of a World at a point in time.
/// </summary>
public sealed class WorldSnapshot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorldSnapshot"/> class.
    /// </summary>
    /// <param name="entityCount">The number of entities in the world.</param>
    /// <param name="entities">A snapshot of entity data (id, type, component count).</param>
    public WorldSnapshot(int entityCount, IReadOnlyList<EntitySnapshot> entities)
    {
        EntityCount = entityCount;
        Entities = entities;
    }

    /// <summary>
    /// Gets a read-only list of entity snapshots.
    /// </summary>
    public IReadOnlyList<EntitySnapshot> Entities { get; }

    /// <summary>
    /// Gets the total number of entities in the world.
    /// </summary>
    public int EntityCount { get; }

    /// <summary>
    /// Represents a snapshot of a single entity.
    /// </summary>
    public sealed class EntitySnapshot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySnapshot"/> class.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <param name="type">The entity type.</param>
        /// <param name="componentCount">The number of components attached to the entity.</param>
        public EntitySnapshot(int id, string type, int componentCount)
        {
            Id = id;
            Type = type;
            ComponentCount = componentCount;
        }

        /// <summary>
        /// Gets the number of components attached to the entity.
        /// </summary>
        public int ComponentCount { get; }

        /// <summary>
        /// Gets the entity ID.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        public string Type { get; }
    }
}
