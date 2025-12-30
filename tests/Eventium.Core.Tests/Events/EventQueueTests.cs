using Eventium.Core.Events;

namespace Eventium.Core.Tests.Events;

public class EventQueueTests
{
    private static readonly EventHandlerDelegate DummyHandler = (_, _) => { };

    [Fact]
    public void Count_AfterMultipleEnqueueDequeue_RemainsAccurate()
    {
        var queue = new EventQueue();
        queue.Enqueue(CreateEvent(1.0, "A"));
        queue.Enqueue(CreateEvent(2.0, "B"));
        Assert.Equal(2, queue.Count);

        queue.Dequeue();
        Assert.Equal(1, queue.Count);

        queue.Enqueue(CreateEvent(3.0, "C"));
        Assert.Equal(2, queue.Count);

        queue.Dequeue();
        queue.Dequeue();
        Assert.Equal(0, queue.Count);
    }

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
    public void Enqueue_LargeNumberOfEvents_MaintainsCorrectOrder()
    {
        var queue = new EventQueue();
        var random = new System.Random(42);

        // Add 1000 events with random times
        var expectedOrder = new List<(double time, string type, int index)>();
        for (int i = 0; i < 1000; i++)
        {
            var time = random.NextDouble() * 100;
            var type = $"EVENT_{i}";
            expectedOrder.Add((time, type, i));
            queue.Enqueue(CreateEvent(time, type));
        }

        // Sort expected order
        expectedOrder.Sort((a, b) =>
        {
            var timeCmp = a.time.CompareTo(b.time);
            if (timeCmp != 0)
            {
                return timeCmp;
            }

            return a.index.CompareTo(b.index); // tie-break by enqueue order
        });

        // Verify dequeue order matches
        for (int i = 0; i < 1000; i++)
        {
            var evt = queue.Dequeue();
            Assert.NotNull(evt);
            Assert.Equal(expectedOrder[i].type, evt.Type);
        }
    }

    [Fact]
    public void Enqueue_MixedTimeAndPriority_OrdersCorrectly()
    {
        var queue = new EventQueue();
        queue.Enqueue(CreateEvent(2.0, "TIME_2_PRI_5", priority: 5));
        queue.Enqueue(CreateEvent(1.0, "TIME_1_PRI_0", priority: 0));
        queue.Enqueue(CreateEvent(2.0, "TIME_2_PRI_1", priority: 1));
        queue.Enqueue(CreateEvent(1.0, "TIME_1_PRI_10", priority: 10));
        queue.Enqueue(CreateEvent(3.0, "TIME_3_PRI_0", priority: 0));

        Assert.Equal("TIME_1_PRI_0", queue.Dequeue()!.Type);
        Assert.Equal("TIME_1_PRI_10", queue.Dequeue()!.Type);
        Assert.Equal("TIME_2_PRI_1", queue.Dequeue()!.Type);
        Assert.Equal("TIME_2_PRI_5", queue.Dequeue()!.Type);
        Assert.Equal("TIME_3_PRI_0", queue.Dequeue()!.Type);
    }

    [Fact]
    public void Enqueue_SameTimeAndPriority_UsesSequenceOrder()
    {
        var queue = new EventQueue();
        var evt1 = CreateEvent(1.0, "FIRST", priority: 5);
        var evt2 = CreateEvent(1.0, "SECOND", priority: 5);
        var evt3 = CreateEvent(1.0, "THIRD", priority: 5);

        queue.Enqueue(evt1);
        queue.Enqueue(evt2);
        queue.Enqueue(evt3);

        Assert.Equal("FIRST", queue.Dequeue()!.Type);
        Assert.Equal("SECOND", queue.Dequeue()!.Type);
        Assert.Equal("THIRD", queue.Dequeue()!.Type);
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
    public void Enqueue_WithNegativePriorities_OrdersCorrectly()
    {
        var queue = new EventQueue();
        var evt1 = CreateEvent(1.0, "LOW", priority: 5);
        var evt2 = CreateEvent(1.0, "HIGH", priority: -10);
        var evt3 = CreateEvent(1.0, "MED", priority: 0);

        queue.Enqueue(evt1);
        queue.Enqueue(evt2);
        queue.Enqueue(evt3);

        Assert.Equal("HIGH", queue.Dequeue()!.Type);
        Assert.Equal("MED", queue.Dequeue()!.Type);
        Assert.Equal("LOW", queue.Dequeue()!.Type);
    }

    [Fact]
    public void Enqueue_WithNegativeTime_WorksCorrectly()
    {
        var queue = new EventQueue();
        var evt1 = CreateEvent(-5.0, "NEGATIVE");
        var evt2 = CreateEvent(0.0, "ZERO");
        var evt3 = CreateEvent(5.0, "POSITIVE");

        queue.Enqueue(evt1);
        queue.Enqueue(evt2);
        queue.Enqueue(evt3);

        Assert.Equal("NEGATIVE", queue.Dequeue()!.Type);
        Assert.Equal("ZERO", queue.Dequeue()!.Type);
        Assert.Equal("POSITIVE", queue.Dequeue()!.Type);
    }

    [Fact]
    public void Enqueue_WithZeroPriority_OrdersByTimeOnly()
    {
        var queue = new EventQueue();
        var evt1 = CreateEvent(2.0, "SECOND", priority: 0);
        var evt2 = CreateEvent(1.0, "FIRST", priority: 0);
        var evt3 = CreateEvent(3.0, "THIRD", priority: 0);

        queue.Enqueue(evt1);
        queue.Enqueue(evt2);
        queue.Enqueue(evt3);

        Assert.Equal("FIRST", queue.Dequeue()!.Type);
        Assert.Equal("SECOND", queue.Dequeue()!.Type);
        Assert.Equal("THIRD", queue.Dequeue()!.Type);
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
