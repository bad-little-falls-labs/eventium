using Eventium.Core.Events;

namespace Eventium.Core.Tests.Events;

public class EventTests
{
    private static readonly EventHandlerDelegate DummyHandler = (_, _) => { };

    [Fact]
    public void CompareTo_EarlierEvent_ReturnsNegative()
    {
        var earlier = new Event(1.0, 0, "A", (IDictionary<string, object?>?)null, DummyHandler);
        var later = new Event(2.0, 0, "B", (IDictionary<string, object?>?)null, DummyHandler);

        Assert.True(earlier.CompareTo(later) < 0);
    }

    [Fact]
    public void CompareTo_LaterEvent_ReturnsPositive()
    {
        var earlier = new Event(1.0, 0, "A", (IDictionary<string, object?>?)null, DummyHandler);
        var later = new Event(2.0, 0, "B", (IDictionary<string, object?>?)null, DummyHandler);

        Assert.True(later.CompareTo(earlier) > 0);
    }

    [Fact]
    public void CompareTo_Null_ReturnsPositive()
    {
        var evt = new Event(1.0, 0, "A", (IDictionary<string, object?>?)null, DummyHandler);

        Assert.True(evt.CompareTo(null) > 0);
    }

    [Fact]
    public void CompareTo_SameTime_ComparesByPriority()
    {
        var lowPriority = new Event(1.0, 1, "A", (IDictionary<string, object?>?)null, DummyHandler);
        var highPriority = new Event(1.0, 2, "B", (IDictionary<string, object?>?)null, DummyHandler);

        Assert.True(lowPriority.CompareTo(highPriority) < 0);
    }

    [Fact]
    public void Constructor_WithDictionaryPayload_SetsProperties()
    {
        var payload = new Dictionary<string, object?> { ["key"] = "value" };

        var evt = new Event(1.5, 2, "TEST_EVENT", payload, DummyHandler);

        Assert.Equal(1.5, evt.Time);
        Assert.Equal(2, evt.Priority);
        Assert.Equal("TEST_EVENT", evt.Type);
        Assert.Equal("value", evt.Payload["key"]);
        Assert.Null(evt.TypedPayload);
    }

    [Fact]
    public void Constructor_WithNullHandler_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Event(1.0, 0, "TEST", new Dictionary<string, object?>(), null!));
    }

    [Fact]
    public void Constructor_WithNullType_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Event(1.0, 0, null!, new Dictionary<string, object?>(), DummyHandler));
    }

    [Fact]
    public void Constructor_WithNullTypedPayload_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Event(1.0, 0, "TEST", (IEventPayload)null!, DummyHandler));
    }

    [Fact]
    public void Constructor_WithTypedPayload_SetsProperties()
    {
        var payload = new TestPayload(42, "test");

        var evt = new Event(2.0, 1, "TYPED_EVENT", payload, DummyHandler);

        Assert.Equal(2.0, evt.Time);
        Assert.Equal(1, evt.Priority);
        Assert.Equal("TYPED_EVENT", evt.Type);
        Assert.Empty(evt.Payload);
        Assert.Same(payload, evt.TypedPayload);
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var evt1 = new Event(1.0, 0, "TEST_A", (IDictionary<string, object?>?)null, DummyHandler);
        var evt2 = new Event(1.0, 0, "TEST_B", (IDictionary<string, object?>?)null, DummyHandler);

        Assert.False(evt1.Equals(evt2));
    }

    [Fact]
    public void Equals_SameTimeAndPriorityAndType_ReturnsTrue()
    {
        var evt1 = new Event(1.0, 0, "TEST", (IDictionary<string, object?>?)null, DummyHandler);
        var evt2 = new Event(1.0, 0, "TEST", (IDictionary<string, object?>?)null, DummyHandler);

        Assert.True(evt1.Equals(evt2));
    }

    [Fact]
    public void GetPayload_WithCorrectType_ReturnsPayload()
    {
        var payload = new TestPayload(42, "test");
        var evt = new Event(1.0, 0, "TEST", payload, DummyHandler);

        var result = evt.GetPayload<TestPayload>();

        Assert.Equal(42, result.Id);
        Assert.Equal("test", result.Name);
    }

    [Fact]
    public void GetPayload_WithDictionaryPayload_ThrowsInvalidCastException()
    {
        var evt = new Event(1.0, 0, "TEST", new Dictionary<string, object?>(), DummyHandler);

        Assert.Throws<InvalidCastException>(() => evt.GetPayload<TestPayload>());
    }

    [Fact]
    public void GetPayload_WithWrongType_ThrowsInvalidCastException()
    {
        var payload = new TestPayload(42, "test");
        var evt = new Event(1.0, 0, "TEST", payload, DummyHandler);

        Assert.Throws<InvalidCastException>(() => evt.GetPayload<OtherPayload>());
    }

    [Fact]
    public void OperatorEquals_Works()
    {
        var evt1 = new Event(1.0, 0, "TEST", (IDictionary<string, object?>?)null, DummyHandler);
        var evt2 = new Event(1.0, 0, "TEST", (IDictionary<string, object?>?)null, DummyHandler);

        Assert.True(evt1 == evt2);
    }

    [Fact]
    public void OperatorLessThan_Works()
    {
        var earlier = new Event(1.0, 0, "A", (IDictionary<string, object?>?)null, DummyHandler);
        var later = new Event(2.0, 0, "B", (IDictionary<string, object?>?)null, DummyHandler);

        Assert.True(earlier < later);
        Assert.False(later < earlier);
    }

    [Fact]
    public void TryGetPayload_WithCorrectType_ReturnsTrueAndPayload()
    {
        var payload = new TestPayload(42, "test");
        var evt = new Event(1.0, 0, "TEST", payload, DummyHandler);

        var success = evt.TryGetPayload<TestPayload>(out var result);

        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal(42, result.Id);
    }

    [Fact]
    public void TryGetPayload_WithWrongType_ReturnsFalseAndNull()
    {
        var payload = new TestPayload(42, "test");
        var evt = new Event(1.0, 0, "TEST", payload, DummyHandler);

        var success = evt.TryGetPayload<OtherPayload>(out var result);

        Assert.False(success);
        Assert.Null(result);
    }

    private sealed record TestPayload(int Id, string Name) : IEventPayload;
    private sealed record OtherPayload(string Data) : IEventPayload;
}
