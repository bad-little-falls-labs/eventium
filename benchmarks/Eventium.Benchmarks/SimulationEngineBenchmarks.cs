// <copyright file="SimulationEngineBenchmarks.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using BenchmarkDotNet.Attributes;
using Eventium.Core;
using Eventium.Core.Events;
using Eventium.Core.Time;

namespace Eventium.Benchmarks;

[MemoryDiagnoser]
[MinColumn]
[MaxColumn]
[MeanColumn]
[MedianColumn]
public class SimulationEngineBenchmarks
{
    [Params(100, 1000, 10000)]
    public int EventCount { get; set; }

    [Benchmark]
    public void RunSimulation_DiscreteTime()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Discrete, step: 1.0), seed: 42);

        for (int i = 0; i < EventCount; i++)
        {
            engine.Schedule(i, $"EVENT_{i}", handler: (_, _) => { /* no-op */ });
        }

        engine.Run();
    }

    [Benchmark]
    public void RunSimulation_NoEventChaining()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous), seed: 42);

        // Schedule all events upfront
        for (int i = 0; i < EventCount; i++)
        {
            engine.Schedule(i, $"EVENT_{i}", handler: (_, _) => { /* no-op */ });
        }

        engine.Run();
    }

    [Benchmark]
    public void RunSimulation_WithEventChaining()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous), seed: 42);
        int remaining = EventCount;

        // First event triggers chain
        engine.Schedule(0, "CHAIN", handler: (ctx, _) =>
        {
            if (remaining > 0)
            {
                remaining--;
                ctx.ScheduleIn(1.0, "CHAIN");
            }
        });

        engine.Run();
    }

    [Benchmark]
    public void RunSimulation_WithMetrics()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous), seed: 42);
        var counter = engine.Metrics.Counter("events_processed");

        for (int i = 0; i < EventCount; i++)
        {
            engine.Schedule(i, $"EVENT_{i}", handler: (ctx, _) =>
            {
                counter.Increment();
            });
        }

        engine.Run();
    }

    [Benchmark]
    public void Schedule_OnlyNoRun()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous), seed: 42);

        for (int i = 0; i < EventCount; i++)
        {
            engine.Schedule(i, $"EVENT_{i}", handler: (_, _) => { /* no-op */ });
        }
    }
}
