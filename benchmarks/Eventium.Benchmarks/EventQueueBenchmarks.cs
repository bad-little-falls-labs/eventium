// <copyright file="EventQueueBenchmarks.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using BenchmarkDotNet.Attributes;
using Eventium.Core.Events;

namespace Eventium.Benchmarks;

[MemoryDiagnoser]
[MinColumn]
[MaxColumn]
[MeanColumn]
[MedianColumn]
public class EventQueueBenchmarks
{
    private static readonly EventHandlerDelegate DummyHandler = (_, _) => { };
    private List<Event>? _events;
    private EventQueue? _queue;

    [Params(100, 1000, 10000)]
    public int EventCount { get; set; }

    [Benchmark]
    public void Dequeue()
    {
        // Pre-populate queue
        var queue = new EventQueue();
        foreach (var evt in _events!)
        {
            queue.Enqueue(evt);
        }

        // Benchmark only dequeue
        while (queue.Count > 0)
        {
            queue.Dequeue();
        }
    }

    [Benchmark]
    public void Enqueue()
    {
        var queue = new EventQueue();
        foreach (var evt in _events!)
        {
            queue.Enqueue(evt);
        }
    }

    [Benchmark]
    public void EnqueueDequeue()
    {
        var queue = new EventQueue();
        foreach (var evt in _events!)
        {
            queue.Enqueue(evt);
        }

        while (queue.Count > 0)
        {
            queue.Dequeue();
        }
    }

    [Benchmark]
    public void PeekTime()
    {
        var queue = new EventQueue();
        foreach (var evt in _events!)
        {
            queue.Enqueue(evt);
        }

        for (int i = 0; i < EventCount; i++)
        {
            _ = queue.PeekTime();
        }
    }

    [GlobalSetup]
    public void Setup()
    {
        _queue = new EventQueue();
        _events = new List<Event>(EventCount);

        var random = new Random(42);
        for (int i = 0; i < EventCount; i++)
        {
            var time = random.NextDouble() * 1000;
            var priority = random.Next(0, 10);
            _events.Add(new Event(time, priority, $"EVENT_{i}", (IDictionary<string, object?>?)null, DummyHandler));
        }
    }
}
