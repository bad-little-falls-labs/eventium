// <copyright file="WorldBenchmarks.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using BenchmarkDotNet.Attributes;
using Eventium.Core.World;

namespace Eventium.Benchmarks;

[MemoryDiagnoser]
[MinColumn]
[MaxColumn]
[MeanColumn]
[MedianColumn]
public partial class WorldBenchmarks
{
    private List<Entity>? _entities;
    private World? _world;

    [Params(100, 1000, 10000)]
    public int EntityCount { get; set; }

    [Benchmark]
    public void AddAndGetEntities()
    {
        var world = new World();
        foreach (var entity in _entities!)
        {
            world.AddEntity(entity);
        }

        for (int i = 0; i < EntityCount; i++)
        {
            _ = world.GetEntity(i);
        }
    }

    [Benchmark]
    public void AddEntities()
    {
        var world = new World();
        foreach (var entity in _entities!)
        {
            world.AddEntity(entity);
        }
    }

    [Benchmark]
    public void CaptureSnapshot()
    {
        // Benchmark snapshot capture with MemoryPack serialization
        _ = _world!.CaptureSnapshot();
    }

    [Benchmark]
    public void GetComponents()
    {
        // Pre-populate world
        var world = new World();
        foreach (var entity in _entities!)
        {
            world.AddEntity(entity);
        }

        // Benchmark component access
        for (int i = 0; i < EntityCount; i++)
        {
            var entity = world.GetEntity(i);
            _ = entity?.GetComponent<TestComponent>("data");
        }
    }

    [Benchmark]
    public void GetEntities()
    {
        // Pre-populate world
        var world = new World();
        foreach (var entity in _entities!)
        {
            world.AddEntity(entity);
        }

        // Benchmark lookups
        for (int i = 0; i < EntityCount; i++)
        {
            _ = world.GetEntity(i);
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        _world = new World();
        _entities = new List<Entity>(EntityCount);

        for (int i = 0; i < EntityCount; i++)
        {
            var entity = new Entity(i, $"ENTITY_{i}");
            entity.AddComponent("data", new TestComponent { Value = i });
            _entities.Add(entity);
        }
    }

    [MemoryPack.MemoryPackable]
    private sealed partial class TestComponent : IComponent
    {
        public int Value { get; set; }
    }
}
