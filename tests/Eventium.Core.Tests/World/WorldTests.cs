using Eventium.Core.World;

namespace Eventium.Core.Tests.World;

public class WorldTests
{

    [Fact]
    public void AddEntity_AddsToEntitiesCollection()
    {
        var world = new Eventium.Core.World.World();
        var entity = new Entity(1, "TEST");

        world.AddEntity(entity);

        Assert.Single(world.Entities);
        Assert.True(world.Entities.ContainsKey(1));
    }

    [Fact]
    public void AddEntity_LargeNumberOfEntities_AllAccessible()
    {
        var world = new Eventium.Core.World.World();

        for (int i = 0; i < 1000; i++)
        {
            world.AddEntity(new Entity(i, $"ENTITY_{i}"));
        }

        Assert.Equal(1000, world.Entities.Count);

        for (int i = 0; i < 1000; i++)
        {
            var entity = world.GetEntity(i);
            Assert.NotNull(entity);
            Assert.Equal($"ENTITY_{i}", entity.Type);
        }
    }

    [Fact]
    public void AddEntity_MultipleEntities_AllAdded()
    {
        var world = new Eventium.Core.World.World();
        var entity1 = new Entity(1, "A");
        var entity2 = new Entity(2, "B");
        var entity3 = new Entity(3, "C");

        world.AddEntity(entity1);
        world.AddEntity(entity2);
        world.AddEntity(entity3);

        Assert.Equal(3, world.Entities.Count);
    }

    [Fact]
    public void AddEntity_SameId_OverwritesExisting()
    {
        var world = new Eventium.Core.World.World();
        var entity1 = new Entity(1, "FIRST");
        var entity2 = new Entity(1, "SECOND");

        world.AddEntity(entity1);
        world.AddEntity(entity2);

        var result = world.GetEntity(1);
        Assert.Equal("SECOND", result!.Type);
    }

    [Fact]
    public void AddEntity_WithComponents_PreservesComponents()
    {
        var world = new Eventium.Core.World.World();
        var entity = new Entity(1, "TEST");
        entity.AddComponent("comp", new TestComponent { Value = 42 });

        world.AddEntity(entity);

        var retrieved = world.GetEntity(1);
        Assert.NotNull(retrieved);
        var component = retrieved.GetComponent<TestComponent>("comp");
        Assert.NotNull(component);
        Assert.Equal(42, component.Value);
    }

    [Fact]
    public void AddEntity_WithNegativeId_Works()
    {
        var world = new Eventium.Core.World.World();
        var entity = new Entity(-1, "NEGATIVE");

        world.AddEntity(entity);

        var result = world.GetEntity(-1);
        Assert.Same(entity, result);
    }

    [Fact]
    public void AddEntity_WithZeroId_Works()
    {
        var world = new Eventium.Core.World.World();
        var entity = new Entity(0, "ZERO");

        world.AddEntity(entity);

        var result = world.GetEntity(0);
        Assert.Same(entity, result);
    }
    [Fact]
    public void Entities_InitiallyEmpty()
    {
        var world = new Eventium.Core.World.World();

        Assert.Empty(world.Entities);
    }

    [Fact]
    public void Entities_IsReadOnly()
    {
        var world = new Eventium.Core.World.World();
        world.AddEntity(new Entity(1, "TEST"));

        // Entities should be a read-only dictionary
        var entities = world.Entities;
        Assert.NotNull(entities);
        Assert.Single(entities);
    }

    [Fact]
    public void GetEntity_EmptyWorld_ReturnsNull()
    {
        var world = new Eventium.Core.World.World();

        var result = world.GetEntity(1);

        Assert.Null(result);
    }

    [Fact]
    public void GetEntity_ExistingId_ReturnsEntity()
    {
        var world = new Eventium.Core.World.World();
        var entity = new Entity(42, "TEST");
        world.AddEntity(entity);

        var result = world.GetEntity(42);

        Assert.Same(entity, result);
    }

    [Fact]
    public void GetEntity_NonexistentId_ReturnsNull()
    {
        var world = new Eventium.Core.World.World();
        world.AddEntity(new Entity(1, "TEST"));

        var result = world.GetEntity(999);

        Assert.Null(result);
    }

    private class TestComponent : IComponent
    {
        public int Value { get; set; }
    }
}
