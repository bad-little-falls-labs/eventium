using Eventium.Core.World;

namespace Eventium.Core.Tests.World;

public class EntityTests
{

    [Fact]
    public void AddComponent_MultipleComponents_AllRetrievable()
    {
        var entity = new Entity(1, "TEST");
        entity.AddComponent("a", new TestComponent { Value = 1 });
        entity.AddComponent("b", new TestComponent { Value = 2 });
        entity.AddComponent("c", new OtherComponent { Name = "test" });

        Assert.Equal(1, entity.GetComponent<TestComponent>("a")!.Value);
        Assert.Equal(2, entity.GetComponent<TestComponent>("b")!.Value);
        Assert.Equal("test", entity.GetComponent<OtherComponent>("c")!.Name);
    }

    [Fact]
    public void AddComponent_SameName_OverwritesExisting()
    {
        var entity = new Entity(1, "TEST");
        entity.AddComponent("test", new TestComponent { Value = 1 });
        entity.AddComponent("test", new TestComponent { Value = 2 });

        var result = entity.GetComponent<TestComponent>("test");

        Assert.NotNull(result);
        Assert.Equal(2, result.Value);
    }

    [Fact]
    public void AddComponent_StoresComponent()
    {
        var entity = new Entity(1, "TEST");
        var component = new TestComponent { Value = 100 };

        entity.AddComponent("test", component);

        var retrieved = entity.GetComponent<TestComponent>("test");
        Assert.NotNull(retrieved);
        Assert.Equal(100, retrieved.Value);
    }
    [Fact]
    public void Constructor_SetsIdAndType()
    {
        var entity = new Entity(42, "PLAYER");

        Assert.Equal(42, entity.Id);
        Assert.Equal("PLAYER", entity.Type);
    }

    [Fact]
    public void GetComponent_NonexistentName_ReturnsNull()
    {
        var entity = new Entity(1, "TEST");

        var result = entity.GetComponent<TestComponent>("missing");

        Assert.Null(result);
    }

    [Fact]
    public void GetComponent_WrongType_ReturnsNull()
    {
        var entity = new Entity(1, "TEST");
        entity.AddComponent("test", new TestComponent { Value = 1 });

        var result = entity.GetComponent<OtherComponent>("test");

        Assert.Null(result);
    }

    private class OtherComponent : IComponent
    {
        public string Name { get; set; } = "";
    }

    private class TestComponent : IComponent
    {
        public int Value { get; set; }
    }
}
