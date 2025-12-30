// <copyright file="SnapshotBufferTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core.Events;
using Eventium.Core.Snapshots;

namespace Eventium.Core.Tests.Snapshots;

public sealed class SnapshotBufferTests
{

    [Fact]
    public void TryGetByTime_ReturnsFalseForNonExactMatch()
    {
        var buffer = new SnapshotBuffer(capacity: 3);
        var snapshot = new TestSnapshot(5.0);

        buffer.Add(snapshot);

        var result = buffer.TryGetByTime(5.1, out var retrieved);

        Assert.False(result);
        Assert.Null(retrieved);
    }

    [Fact]
    public void TryGetByTime_StillWorksForExactMatches()
    {
        var buffer = new SnapshotBuffer(capacity: 3);
        var snapshot = new TestSnapshot(5.0);

        buffer.Add(snapshot);

        var result = buffer.TryGetByTime(5.0, out var retrieved);

        Assert.True(result);
        Assert.Same(snapshot, retrieved);
    }

    [Fact]
    public void TryGetLatestAtOrBefore_AfterOverwrite_FindsCorrectSnapshot()
    {
        var buffer = new SnapshotBuffer(capacity: 2);
        var snapshot1 = new TestSnapshot(1.0);
        var snapshot2 = new TestSnapshot(5.0);
        var snapshot3 = new TestSnapshot(10.0); // This will overwrite snapshot1

        buffer.Add(snapshot1);
        buffer.Add(snapshot2);
        buffer.Add(snapshot3); // Overwrites oldest (snapshot1)

        // Should not find snapshot1 anymore
        var result = buffer.TryGetLatestAtOrBefore(3.0, out var snapshot);
        Assert.False(result); // No snapshot at or before 3.0 (1.0 was overwritten)

        // Should find snapshot2 for target 7.5
        result = buffer.TryGetLatestAtOrBefore(7.5, out snapshot);
        Assert.True(result);
        Assert.Same(snapshot2, snapshot);

        // Should find snapshot3 for target 15.0
        result = buffer.TryGetLatestAtOrBefore(15.0, out snapshot);
        Assert.True(result);
        Assert.Same(snapshot3, snapshot);
    }

    [Fact]
    public void TryGetLatestAtOrBefore_ExactMatch_ReturnsExactSnapshot()
    {
        var buffer = new SnapshotBuffer(capacity: 3);
        var snapshot1 = new TestSnapshot(1.0);
        var snapshot2 = new TestSnapshot(5.0);
        var snapshot3 = new TestSnapshot(10.0);

        buffer.Add(snapshot1);
        buffer.Add(snapshot2);
        buffer.Add(snapshot3);

        var result = buffer.TryGetLatestAtOrBefore(5.0, out var snapshot);

        Assert.True(result);
        Assert.Same(snapshot2, snapshot);
    }

    [Fact]
    public void TryGetLatestAtOrBefore_NoExactMatch_ReturnsLatestBeforeTarget()
    {
        var buffer = new SnapshotBuffer(capacity: 3);
        var snapshot1 = new TestSnapshot(1.0);
        var snapshot2 = new TestSnapshot(5.0);
        var snapshot3 = new TestSnapshot(10.0);

        buffer.Add(snapshot1);
        buffer.Add(snapshot2);
        buffer.Add(snapshot3);

        // Target time 7.5 should return snapshot at 5.0
        var result = buffer.TryGetLatestAtOrBefore(7.5, out var snapshot);

        Assert.True(result);
        Assert.Same(snapshot2, snapshot);
    }
    [Fact]
    public void TryGetLatestAtOrBefore_NoSnapshots_ReturnsFalse()
    {
        var buffer = new SnapshotBuffer(capacity: 3);

        var result = buffer.TryGetLatestAtOrBefore(5.0, out var snapshot);

        Assert.False(result);
        Assert.Null(snapshot);
    }

    [Fact]
    public void TryGetLatestAtOrBefore_TargetAfterAllSnapshots_ReturnsLatestSnapshot()
    {
        var buffer = new SnapshotBuffer(capacity: 3);
        var snapshot1 = new TestSnapshot(1.0);
        var snapshot2 = new TestSnapshot(5.0);
        var snapshot3 = new TestSnapshot(10.0);

        buffer.Add(snapshot1);
        buffer.Add(snapshot2);
        buffer.Add(snapshot3);

        var result = buffer.TryGetLatestAtOrBefore(20.0, out var snapshot);

        Assert.True(result);
        Assert.Same(snapshot3, snapshot);
    }

    [Fact]
    public void TryGetLatestAtOrBefore_TargetBeforeAllSnapshots_ReturnsFalse()
    {
        var buffer = new SnapshotBuffer(capacity: 3);
        var snapshot1 = new TestSnapshot(5.0);
        var snapshot2 = new TestSnapshot(10.0);

        buffer.Add(snapshot1);
        buffer.Add(snapshot2);

        var result = buffer.TryGetLatestAtOrBefore(3.0, out var snapshot);

        Assert.False(result);
        Assert.Null(snapshot);
    }

    private sealed class TestSnapshot : ISimulationSnapshot
    {
        public TestSnapshot(double time)
        {
            Time = time;
            World = new WorldSnapshot(0, Array.Empty<WorldSnapshot.EntitySnapshot>());
            Queue = new QueueSnapshot(0, null);
            Rng = new RngState(time, 0);
        }

        public int EventsProcessed => 0;

        public QueueSnapshot Queue { get; }

        public RngState Rng { get; }

        public double Time { get; }

        public WorldSnapshot World { get; }
    }
}
