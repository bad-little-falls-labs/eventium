// <copyright file="DeterminismInvariantTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Eventium.Core;
using Eventium.Core.Events;
using Eventium.Core.Random;
using Eventium.Core.Snapshots;
using Eventium.Core.Time;
using Eventium.Core.World;
using Xunit;
using SimWorld = Eventium.Core.World.World;

namespace Eventium.Core.Tests.Integration;

/// <summary>
/// End-to-end determinism and snapshot invariants.
/// </summary>
public sealed partial class DeterminismInvariantTests
{
    private const string Tick = "TICK";

    private static readonly JsonSerializerOptions HashOptions = new()
    {
        WriteIndented = false
    };

    [Fact]
    public void DeterministicOrdering_ReplaysIdentically()
    {
        var first = RunScenario(seed: 1337, maxEvents: 30);
        var second = RunScenario(seed: 1337, maxEvents: 30);

        Assert.Equal(first.Trace, second.Trace);
        Assert.Equal(first.WorldHash, second.WorldHash);
    }

    [Fact]
    public void QueueSnapshot_RestoresFuture()
    {
        var harness = BuildHarness(seed: 2025);
        var snapshot = (SimulationSnapshot)harness.Engine.CaptureSnapshot();

        var firstAdvance = harness.ProcessEvents(steps: 6);
        var finalWorldHash = HashWorld(harness.Engine.World.CaptureSnapshot());
        var finalQueueHash = HashQueue(((IStatefulEventQueue)harness.Engine.Queue).CaptureSnapshot());

        harness.Engine.RestoreSnapshot(snapshot);
        harness.ProcessEvents(steps: 6);

        var replayWorldHash = HashWorld(harness.Engine.World.CaptureSnapshot());
        var replayQueueHash = HashQueue(((IStatefulEventQueue)harness.Engine.Queue).CaptureSnapshot());

        Assert.Equal(firstAdvance.EventsProcessed, harness.Engine.EventsProcessed - snapshot.EventsProcessed);
        Assert.Equal(finalWorldHash, replayWorldHash);
        Assert.Equal(finalQueueHash, replayQueueHash);
    }

    [Fact]
    public void RngState_RestoresAfterSnapshot()
    {
        var result = RunRngCheckpointScenario(seed: 99, totalEvents: 12, snapshotAfter: 5);

        Assert.Equal(result.CheckpointsAfterSnapshot, result.ReplayedCheckpoints);
    }

    [Fact]
    public void WorldSnapshot_IsolatedFromMutations()
    {
        var world = new SimWorld();
        var entity = new Entity(1, "agent");
        entity.AddComponent("state", new StateComponent { Value = 10, Label = "initial" });
        world.AddEntity(entity);
        world.Globals["energy"] = 5;

        var snapshot = world.CaptureSnapshot();

        // Mutate world heavily after snapshot
        entity.GetComponent<StateComponent>("state")!.Value = 999;
        entity.AddComponent("extra", new StateComponent { Value = -1, Label = "mutated" });
        world.Globals["energy"] = 42;

        world.RestoreSnapshot(snapshot);

        var restoredHash = HashWorld(world.CaptureSnapshot());
        var snapshotHash = HashWorld(snapshot);

        Assert.Equal(snapshotHash, restoredHash);
        Assert.Null(world.GetEntity(1)!.GetComponent<StateComponent>("extra"));
        var restoredState = world.GetEntity(1)!.GetComponent<StateComponent>("state")!;
        Assert.Equal(10, restoredState.Value);
        Assert.Equal("initial", restoredState.Label);
    }

    private static SimulationHarness BuildHarness(int seed)
    {
        var timeModel = new TimeModel(TimeMode.Discrete, step: 1.0);
        var world = new SimWorld();
        var queue = new EventQueue();
        var rng = new RewindableRandomSource(seed);
        var engine = new SimulationEngine(timeModel, world, queue, rng);

        var harness = new SimulationHarness(engine);
        harness.SeedWorld();
        harness.SeedEvents();
        return harness;
    }

    private static string HashPayload(Event evt)
    {
        if (evt.TypedPayload is not null)
        {
            return HashString(JsonSerializer.Serialize(evt.TypedPayload, evt.TypedPayload.GetType(), HashOptions));
        }

        return HashString(JsonSerializer.Serialize(evt.Payload, HashOptions));
    }

    private static string HashQueue(QueueSnapshot snapshot)
    {
        var normalized = snapshot.Events
            .OrderBy(e => e.Time)
            .ThenBy(e => e.Priority)
            .ThenBy(e => e.Sequence)
            .Select(e => new
            {
                e.Time,
                e.Priority,
                e.Sequence,
                e.Type,
                Payload = HashPayload(e)
            });

        var json = JsonSerializer.Serialize(new { snapshot.NextSequence, Events = normalized }, HashOptions);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json)));
    }

    private static string HashString(string input)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)));
    }

    private static string HashWorld(WorldSnapshot snapshot)
    {
        var json = JsonSerializer.Serialize(snapshot, HashOptions);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    private static RngReplayResult RunRngCheckpointScenario(int seed, int totalEvents, int snapshotAfter)
    {
        var harness = BuildHarness(seed);
        var checkpoints = new List<(double delta, double dt)>();
        var replayCheckpoints = new List<(double delta, double dt)>();
        SimulationSnapshot? snapshot = null;

        harness.EventCallback = (index, payload, delta, dt, ctx) =>
        {
            if (index == snapshotAfter)
            {
                snapshot = (SimulationSnapshot)harness.Engine.CaptureSnapshot();
            }

            checkpoints.Add((delta, dt));
        };

        harness.ProcessEvents(steps: totalEvents);

        Assert.NotNull(snapshot);

        harness.Engine.RestoreSnapshot(snapshot!);
        harness.ResetTrace();

        harness.EventCallback = (_, payload, delta, dt, ctx) => replayCheckpoints.Add((delta, dt));
        harness.ProcessEvents(steps: totalEvents - snapshotAfter);

        return new RngReplayResult(checkpoints.Skip(snapshotAfter).ToList(), replayCheckpoints);
    }

    private static ScenarioResult RunScenario(int seed, int maxEvents)
    {
        var harness = BuildHarness(seed);
        harness.ProcessEvents(steps: maxEvents);

        var worldHash = HashWorld(harness.Engine.World.CaptureSnapshot());
        return new ScenarioResult(harness.Trace, worldHash);
    }

    private sealed class SimulationHarness
    {
        private readonly List<TraceEntry> _trace = new();
        private int _eventIndex;

        public SimulationHarness(SimulationEngine engine)
        {
            Engine = engine;
            Engine.RegisterHandler(Tick, HandleTick);
        }

        public SimulationEngine Engine { get; }

        public Action<int, TickPayload, double, double, ISimulationContext>? EventCallback { get; set; }

        public IReadOnlyList<TraceEntry> Trace => _trace;

        public SimulationStepResult ProcessEvents(int steps)
        {
            return Engine.ProcessUntil(maxEvents: steps);
        }

        public void ResetTrace()
        {
            _trace.Clear();
            _eventIndex = 0;
        }

        public void SeedEvents()
        {
            for (int i = 0; i < 3; i++)
            {
                var payload = new TickPayload(i + 1, Step: 0, Delta: 0);
                Engine.Schedule(
                    time: 0,
                    type: Tick,
                    payload: payload,
                    priority: 0);
            }
        }

        public void SeedWorld()
        {
            for (int i = 0; i < 3; i++)
            {
                var entity = new Entity(i + 1, "agent");
                entity.AddComponent("state", new StateComponent { Value = 0, Label = $"a{i + 1}" });
                Engine.World.AddEntity(entity);
            }

            Engine.World.Globals["seed"] = "baseline";
        }

        private void HandleTick(ISimulationContext context, Event evt)
        {
            _eventIndex++;
            var payload = evt.GetPayload<TickPayload>();

            var entity = context.World.GetEntity(payload.EntityId)!;
            var state = entity.GetComponent<StateComponent>("state")!;

            // Apply payload
            state.Value += payload.Delta;

            var logEntry = new TraceEntry(evt.Time, evt.Type, payload.EntityId, payload.Step, HashString(JsonSerializer.Serialize(payload, HashOptions)));
            _trace.Add(logEntry);

            // Schedule next tick with deterministic randomness
            if (payload.Step >= 4)
            {
                return;
            }

            var nextDelta = context.Rng.NextDouble();
            var dt = context.Rng.NextDouble() * 0.5;
            var nextPayload = payload with { Step = payload.Step + 1, Delta = nextDelta };

            var nextPriority = payload.Step % 2;
            EventCallback?.Invoke(_eventIndex, payload, nextDelta, dt, context);
            context.Schedule(
                time: evt.Time + dt,
                type: Tick,
                payload: nextPayload,
                priority: nextPriority);
        }
    }

    [MemoryPack.MemoryPackable]
    private sealed partial class StateComponent : IComponent
    {

        public string? Label { get; set; }
        public double Value { get; set; }
    }

    private sealed record RngReplayResult(IReadOnlyList<(double delta, double dt)> CheckpointsAfterSnapshot, IReadOnlyList<(double delta, double dt)> ReplayedCheckpoints);

    private sealed record ScenarioResult(IReadOnlyList<TraceEntry> Trace, string WorldHash);

    private sealed record TickPayload(int EntityId, int Step, double Delta) : IEventPayload;

    private sealed record TraceEntry(double Time, string Type, int EntityId, int Step, string PayloadHash);
}
