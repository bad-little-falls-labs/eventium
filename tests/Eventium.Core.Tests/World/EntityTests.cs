using Eventium.Core.World;

namespace Eventium.Core.Tests.World;

public partial class EntityTests
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
    public void AddComponent_MultipleNamed_CanRetrieveIndividually()
    {
        var entity = new Entity(1, "TEST");
        entity.AddComponent("a", new TestComponent { Value = 1 });
        entity.AddComponent("b", new TestComponent { Value = 2 });

        Assert.NotNull(entity.GetComponent<TestComponent>("a"));
        Assert.NotNull(entity.GetComponent<TestComponent>("b"));
        Assert.Equal(1, entity.GetComponent<TestComponent>("a")!.Value);
        Assert.Equal(2, entity.GetComponent<TestComponent>("b")!.Value);
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
    public void AddComponent_WithEmptyName_Works()
    {
        var entity = new Entity(1, "TEST");
        var component = new TestComponent { Value = 42 };

        entity.AddComponent("", component);

        var result = entity.GetComponent<TestComponent>("");
        Assert.NotNull(result);
        Assert.Equal(42, result.Value);
    }
    [Fact]
    public void Constructor_SetsIdAndType()
    {
        var entity = new Entity(42, "PLAYER");

        Assert.Equal(42, entity.Id);
        Assert.Equal("PLAYER", entity.Type);
    }

    [Fact]
    public void GetComponent_AfterOverwrite_ReturnsNewValue()
    {
        var entity = new Entity(1, "TEST");
        entity.AddComponent("test", new TestComponent { Value = 1 });
        entity.AddComponent("test", new TestComponent { Value = 99 });

        var result = entity.GetComponent<TestComponent>("test");

        Assert.NotNull(result);
        Assert.Equal(99, result.Value);
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

    [MemoryPack.MemoryPackable]
    private sealed partial class OtherComponent : IComponent
    {
        public string Name { get; set; } = "";
    }

    [MemoryPack.MemoryPackable]
    private sealed partial class TestComponent : IComponent
    {
        public int Value { get; set; }
    }
}
