// <copyright file="IComponent.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
namespace Eventium.Core.World;

/// <summary>
/// Marker interface for entity components.
/// Domain-specific components implement this.
/// Components must be serializable with MemoryPack for snapshot capture.
/// Add [MemoryPackable] attribute to custom component classes.
/// </summary>
public interface IComponent
{
}
