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
    public void Entities_InitiallyEmpty()
    {
        var world = new Eventium.Core.World.World();

        Assert.Empty(world.Entities);
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
}
