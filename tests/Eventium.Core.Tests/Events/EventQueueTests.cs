using Eventium.Core.Events;

namespace Eventium.Core.Tests.Events;

public class EventQueueTests
{
    private static readonly EventHandlerDelegate DummyHandler = (_, _) => { };

    [Fact]
    public void Count_EmptyQueue_ReturnsZero()
    {
        var queue = new EventQueue();

        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void Dequeue_EmptyQueue_ReturnsNull()
    {
        var queue = new EventQueue();

        var result = queue.Dequeue();

        Assert.Null(result);
    }

    [Fact]
    public void Dequeue_MultipleEvents_ReturnsInTimeOrder()
    {
        var queue = new EventQueue();
        var evt1 = CreateEvent(3.0, "THIRD");
        var evt2 = CreateEvent(1.0, "FIRST");
        var evt3 = CreateEvent(2.0, "SECOND");

        queue.Enqueue(evt1);
        queue.Enqueue(evt2);
        queue.Enqueue(evt3);

        Assert.Equal("FIRST", queue.Dequeue()!.Type);
        Assert.Equal("SECOND", queue.Dequeue()!.Type);
        Assert.Equal("THIRD", queue.Dequeue()!.Type);
    }

    [Fact]
    public void Dequeue_SameTime_ReturnsInPriorityOrder()
    {
        var queue = new EventQueue();
        var evt1 = CreateEvent(1.0, "LOW", priority: 10);
        var evt2 = CreateEvent(1.0, "HIGH", priority: 1);
        var evt3 = CreateEvent(1.0, "MED", priority: 5);

        queue.Enqueue(evt1);
        queue.Enqueue(evt2);
        queue.Enqueue(evt3);

        Assert.Equal("HIGH", queue.Dequeue()!.Type);
        Assert.Equal("MED", queue.Dequeue()!.Type);
        Assert.Equal("LOW", queue.Dequeue()!.Type);
    }

    [Fact]
    public void Dequeue_SingleEvent_ReturnsEventAndDecrementsCount()
    {
        var queue = new EventQueue();
        var evt = CreateEvent(1.0, "TEST");
        queue.Enqueue(evt);

        var result = queue.Dequeue();

        Assert.Same(evt, result);
        Assert.Equal(0, queue.Count);
    }

    [Fact]
    public void Enqueue_SingleEvent_IncrementsCount()
    {
        var queue = new EventQueue();
        var evt = CreateEvent(1.0, "TEST");

        queue.Enqueue(evt);

        Assert.Equal(1, queue.Count);
    }

    [Fact]
    public void PeekTime_DoesNotRemoveEvent()
    {
        var queue = new EventQueue();
        queue.Enqueue(CreateEvent(1.0, "TEST"));

        queue.PeekTime();

        Assert.Equal(1, queue.Count);
    }

    [Fact]
    public void PeekTime_EmptyQueue_ReturnsNull()
    {
        var queue = new EventQueue();

        var result = queue.PeekTime();

        Assert.Null(result);
    }

    [Fact]
    public void PeekTime_WithEvents_ReturnsEarliestTime()
    {
        var queue = new EventQueue();
        queue.Enqueue(CreateEvent(5.0, "LATER"));
        queue.Enqueue(CreateEvent(2.0, "EARLIER"));

        var result = queue.PeekTime();

        Assert.Equal(2.0, result);
    }

    private static Event CreateEvent(double time, string type, int priority = 0)
    {
        return new Event(time, priority, type, (IDictionary<string, object?>?)null, DummyHandler);
    }
}
