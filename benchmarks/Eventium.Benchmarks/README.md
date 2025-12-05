# Eventium Benchmarks

This project contains performance benchmarks for the Eventium simulation engine using BenchmarkDotNet.

## Running Benchmarks

Run all benchmarks:

```bash
cd benchmarks/Eventium.Benchmarks
dotnet run -c Release
```

Run specific benchmark:

```bash
dotnet run -c Release --filter "*EventQueue*"
dotnet run -c Release --filter "*SimulationEngine*"
dotnet run -c Release --filter "*World*"
```

Run with custom configuration:

```bash
# Short run for quick feedback
dotnet run -c Release -- --job short

# Filter specific benchmarks
dotnet run -c Release -- --filter "*Enqueue*"

# Export results to different formats
dotnet run -c Release -- --exporters json,html
```

## Manual Execution Guide

- Build first to ensure dependencies are restored:

```bash
cd /Users/jimcorrell/Development/neho/eventium
dotnet build benchmarks/Eventium.Benchmarks/Eventium.Benchmarks.csproj -c Release
```

- Run a single benchmark method by full name:

```bash
cd benchmarks/Eventium.Benchmarks
dotnet run -c Release --filter "Eventium.Benchmarks.EventQueueBenchmarks.Enqueue*"
```

- List all available benchmarks without running:

```bash
dotnet run -c Release -- --list flat
```

- Export results artifacts (CSV/HTML/Markdown/JSON) locally:

```bash
dotnet run -c Release -- --exporters csv,html,github,json
# Artifacts are saved under `benchmarks/Eventium.Benchmarks/BenchmarkDotNet.Artifacts/results/`
```

- Use categories or namespaces to narrow runs:

```bash
# By class
dotnet run -c Release --filter "Eventium.Benchmarks.WorldBenchmarks*"
# By method pattern
dotnet run -c Release --filter "*GetEntities*"
```

- Troubleshooting on macOS (permission message):

If you see "Failed to set up high priority (Permission denied)", it's safe to ignore for local runs. For stricter isolation:

```bash
# Disable high-priority attempts and affinity
dotnet run -c Release -- --disableLogFile --affinity 1
```

- Faster iteration with fewer iterations (less precise):

```bash
dotnet run -c Release -- --job short --warmupCount 1 --iterationCount 3
```

## Benchmark Categories

### EventQueue Benchmarks

- `Enqueue` - Measures event queue insertion performance
- `Dequeue` - Measures event queue removal performance
- `EnqueueDequeue` - Full cycle performance
- `PeekTime` - Peek operation performance

### SimulationEngine Benchmarks

- `RunSimulation_NoEventChaining` - Basic event processing
- `RunSimulation_WithEventChaining` - Dynamic event scheduling
- `RunSimulation_WithMetrics` - Metrics collection overhead
- `RunSimulation_DiscreteTime` - Discrete time mode performance
- `Schedule_OnlyNoRun` - Event scheduling overhead

### World Benchmarks

- `AddEntities` - Entity addition performance
- `GetEntities` - Entity lookup performance
- `AddAndGetEntities` - Combined operations
- `GetComponents` - Component access performance

## Interpreting Results

BenchmarkDotNet provides several metrics:

- **Mean**: Average execution time
- **Median**: Middle value (less affected by outliers)
- **Min/Max**: Range of execution times
- **StdDev**: Standard deviation
- **Allocated**: Memory allocations per operation

## Baseline Performance Targets

| Benchmark                        | Target (Mean) | Notes                         |
| -------------------------------- | ------------- | ----------------------------- |
| EventQueue.Enqueue (1K events)   | < 100 μs      | Fast priority queue insertion |
| SimulationEngine.Run (1K events) | < 1 ms        | Event processing overhead     |
| World.GetEntity (1K entities)    | < 50 μs       | Fast entity lookups           |

## CI Integration

Benchmarks can be run in CI to track performance regressions. See `.github/workflows/benchmarks.yml` for the automated workflow.
