// <copyright file="SnapshotTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.Snapshots;
using Xunit;

namespace Eventium.Core.Tests.Snapshots;

/// <summary>
/// Tests for snapshot types and ring buffer functionality.
/// </summary>
public sealed class SnapshotTests
{

    [Fact]
    public void QueueSnapshot_AllowsEmptyQueue()
    {
        var snapshot = new QueueSnapshot(count: 0, nextEventTime: null);

        Assert.Equal(0, snapshot.Count);
        Assert.Null(snapshot.NextEventTime);
    }

    [Fact]
    public void QueueSnapshot_CapturesQueueState()
    {
        var snapshot = new QueueSnapshot(count: 42, nextEventTime: 3.5);

        Assert.Equal(42, snapshot.Count);
        Assert.Equal(3.5, snapshot.NextEventTime);
    }

    [Fact]
    public void RngState_CapturesMetadata()
    {
        var rng = new RngState(captureTime: 5.0, eventCount: 42);

        Assert.Equal(5.0, rng.CaptureTime);
        Assert.Equal(42, rng.EventCount);
    }

    [Fact]
    public void SimulationSnapshot_CombinesAllComponents()
    {
        var world = new WorldSnapshot(1, new List<WorldSnapshot.EntitySnapshot> { new(1, "Test", 2) });
        var queue = new QueueSnapshot(5, 10.0);
        var rng = new RngState(5.0, 100);

        var snapshot = new SimulationSnapshot(time: 5.0, eventsProcessed: 100, world, queue, rng);

        Assert.Equal(5.0, snapshot.Time);
        Assert.Equal(100, snapshot.EventsProcessed);
        Assert.Same(world, snapshot.World);
        Assert.Same(queue, snapshot.Queue);
        Assert.Same(rng, snapshot.Rng);
    }

    [Fact]
    public void SnapshotBuffer_AddsSnapshots()
    {
        var buffer = new SnapshotBuffer(10);
        var snapshot = CreateTestSnapshot(1.0, 10);

        buffer.Add(snapshot);

        Assert.Equal(1, buffer.Count);
    }

    [Fact]
    public void SnapshotBuffer_Clear()
    {
        var buffer = new SnapshotBuffer(10);

        buffer.Add(CreateTestSnapshot(1.0, 10));
        buffer.Add(CreateTestSnapshot(2.0, 20));

        Assert.Equal(2, buffer.Count);

        buffer.Clear();

        Assert.Equal(0, buffer.Count);
        Assert.False(buffer.TryGetByTime(1.0, out _));
    }

    [Fact]
    public void SnapshotBuffer_RejectsNullSnapshot()
    {
        var buffer = new SnapshotBuffer(10);

        var ex = Assert.Throws<ArgumentNullException>(() => buffer.Add(null!));

        Assert.Equal("snapshot", ex.ParamName);
    }

    [Fact]
    public void SnapshotBuffer_RetrievesByTime()
    {
        var buffer = new SnapshotBuffer(10);
        var snapshot1 = CreateTestSnapshot(1.0, 10);
        var snapshot2 = CreateTestSnapshot(2.0, 20);

        buffer.Add(snapshot1);
        buffer.Add(snapshot2);

        Assert.True(buffer.TryGetByTime(1.0, out var retrieved1));
        Assert.Equal(1.0, retrieved1!.Time);
        Assert.Equal(10, retrieved1.EventsProcessed);

        Assert.True(buffer.TryGetByTime(2.0, out var retrieved2));
        Assert.Equal(2.0, retrieved2!.Time);
    }

    [Fact]
    public void SnapshotBuffer_ReturnsAllInChronologicalOrder()
    {
        var buffer = new SnapshotBuffer(10);

        buffer.Add(CreateTestSnapshot(3.0, 30));
        buffer.Add(CreateTestSnapshot(1.0, 10));
        buffer.Add(CreateTestSnapshot(2.0, 20));

        var all = buffer.GetAll();

        Assert.Equal(3, all.Count);
        Assert.Equal(1.0, all[0].Time);
        Assert.Equal(2.0, all[1].Time);
        Assert.Equal(3.0, all[2].Time);
    }

    [Fact]
    public void SnapshotBuffer_ReturnsFalseForMissingTime()
    {
        var buffer = new SnapshotBuffer(10);
        var snapshot = CreateTestSnapshot(1.0, 10);

        buffer.Add(snapshot);

        Assert.False(buffer.TryGetByTime(3.0, out var retrieved));
        Assert.Null(retrieved);
    }

    [Fact]
    public void SnapshotBuffer_StartsEmpty()
    {
        var buffer = new SnapshotBuffer(10);

        Assert.Equal(0, buffer.Count);
        Assert.Equal(10, buffer.Capacity);
    }

    [Fact]
    public void SnapshotBuffer_ValidatesCapacity()
    {
        var ex = Assert.Throws<ArgumentException>(() => new SnapshotBuffer(0));

        Assert.Contains("at least 1", ex.Message);
    }

    [Fact]
    public void SnapshotBuffer_WrapsWhenFull()
    {
        var buffer = new SnapshotBuffer(3);

        buffer.Add(CreateTestSnapshot(1.0, 10));
        buffer.Add(CreateTestSnapshot(2.0, 20));
        buffer.Add(CreateTestSnapshot(3.0, 30));

        Assert.Equal(3, buffer.Count);

        // Add fourth snapshot, oldest should be overwritten
        buffer.Add(CreateTestSnapshot(4.0, 40));

        Assert.Equal(3, buffer.Count);
        Assert.False(buffer.TryGetByTime(1.0, out _));
        Assert.True(buffer.TryGetByTime(4.0, out _));
    }

    [Fact]
    public void SnapshotPolicy_AcceptsBothIntervals()
    {
        var policy = new SnapshotPolicy(eventInterval: 100, timeInterval: 5.0, maxSnapshots: 50);

        Assert.Equal(100, policy.EventInterval);
        Assert.Equal(5.0, policy.TimeInterval);
        Assert.Equal(50, policy.MaxSnapshots);
    }

    [Fact]
    public void SnapshotPolicy_AcceptsEventInterval()
    {
        var policy = new SnapshotPolicy(eventInterval: 1000);

        Assert.Equal(1000, policy.EventInterval);
        Assert.Null(policy.TimeInterval);
        Assert.Equal(100, policy.MaxSnapshots);
    }

    [Fact]
    public void SnapshotPolicy_AcceptsTimeInterval()
    {
        var policy = new SnapshotPolicy(timeInterval: 10.0);

        Assert.Null(policy.EventInterval);
        Assert.Equal(10.0, policy.TimeInterval);
    }
    [Fact]
    public void SnapshotPolicy_RequiresAtLeastOneInterval()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new SnapshotPolicy(eventInterval: null, timeInterval: null));

        Assert.Contains("At least one", ex.Message);
    }

    [Fact]
    public void SnapshotPolicy_ValidatesMaxSnapshots()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new SnapshotPolicy(eventInterval: 100, maxSnapshots: 0));

        Assert.Contains("at least 1", ex.Message);
    }

    [Fact]
    public void WorldSnapshot_CapturesEntityData()
    {
        var entities = new List<WorldSnapshot.EntitySnapshot>
        {
            new(1, "Agent", 3),
            new(2, "Resource", 1)
        };

        var snapshot = new WorldSnapshot(2, entities);

        Assert.Equal(2, snapshot.EntityCount);
        Assert.Equal(2, snapshot.Entities.Count);
        Assert.Equal("Agent", snapshot.Entities[0].Type);
    }

    private static ISimulationSnapshot CreateTestSnapshot(double time, int eventsProcessed)
    {
        var world = new WorldSnapshot(0, new List<WorldSnapshot.EntitySnapshot>());
        var queue = new QueueSnapshot(0, null);
        var rng = new RngState(time, eventsProcessed);

        return new SimulationSnapshot(time, eventsProcessed, world, queue, rng);
    }
}
