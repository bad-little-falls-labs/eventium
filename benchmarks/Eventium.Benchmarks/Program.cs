// <copyright file="Program.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using BenchmarkDotNet.Running;

namespace Eventium.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        // Run all benchmarks or specific ones based on command line args
        if (args.Length > 0)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
        else
        {
            // Default: run all benchmarks
            BenchmarkRunner.Run<EventQueueBenchmarks>();
            BenchmarkRunner.Run<SimulationEngineBenchmarks>();
            BenchmarkRunner.Run<WorldBenchmarks>();
        }
    }
}
