# Copilot Instructions for Eventium

## Project Overview

Eventium is a modern, extensible event-driven simulation engine for modeling real-world systems, processes, and scenarios.
It supports both turn-based (discrete) and continuous-time (timed) simulations through a unified scheduling and event-processing architecture.

## Technology Stack

- **.NET 9.0** with C# latest language features
- **xUnit** for unit testing
- **Nullable reference types** enabled globally
- **Implicit usings** enabled

## Building and Testing

```bash
# Build the solution
dotnet build

# Run all tests
dotnet test

# Check code formatting
dotnet format --verify-no-changes

# Auto-fix formatting issues
dotnet format

# Run sample scenarios
dotnet run --project src/Eventium.Scenarios
```

## Code Style Guidelines

- Use **file-scoped namespaces** (e.g., `namespace Eventium.Core.Engine;`)
- Use **4 spaces** for indentation in C# files
- Include **XML documentation comments** on all public types and members
- Add **copyright headers** at the top of each C# file:
  ```csharp
  // <copyright file="SimulationEngine.cs" company="bad-little-falls-labs">
  // Copyright © 2025 bad-little-falls-labs. All rights reserved.
  // </copyright>
  ```
- Prefer **records** for immutable data types implementing `IEventPayload`
- Use **primary constructors** where appropriate
- Files should end with a newline character
- Use LF line endings

## Project Structure

```
eventium/
├── src/
│   ├── Eventium.Core/          # Core simulation engine library
│   │   ├── Engine/             # SimulationEngine and context interfaces
│   │   ├── Events/             # Event types and queue implementation
│   │   ├── Instrumentation/    # Metrics and logging
│   │   ├── Random/             # RNG abstractions
│   │   ├── Systems/            # System interfaces for event handling
│   │   ├── Time/               # Time model (Discrete/Continuous)
│   │   └── World/              # Entity-component world model
│   └── Eventium.Scenarios/     # Sample simulation scenarios
├── tests/
│   └── Eventium.Core.Tests/    # Unit tests mirroring Core structure
└── docs/                       # Documentation
```

## Testing Conventions

- Test files should mirror the structure of the source files
- Use **[Fact]** for single test cases
- Use **[Theory]** with **[InlineData]** for parameterized tests
- Test class names should end with `Tests` (e.g., `SimulationEngineTests`)
- Test methods should follow `MethodName_Condition_ExpectedResult` naming

## Key Abstractions

- **ISimulationEngine / SimulationEngine**: Central orchestrator that manages event scheduling and execution
- **IEventQueue / EventQueue**: Priority queue for events ordered by time and priority
- **IWorld / World**: Container for entities with component-based state
- **ISystem**: Plug-in interface for custom simulation logic that handles specific event types
- **Event**: Immutable event with time, type, payload, and handler
- **IEventPayload**: Marker interface for typed event payloads
- **TimeModel**: Configuration for discrete (turn-based) or continuous time modes

## Common Patterns

### Scheduling Events

```csharp
// Schedule at absolute time
engine.Schedule(time: 5.0, type: "MY_EVENT", payload: new MyPayload(data));

// Schedule relative to current time
engine.ScheduleIn(dt: 3.0, type: "DELAYED_EVENT");
```

### Creating Systems

```csharp
public sealed class MySystem : ISystem
{
    public IEnumerable<string> HandledEventTypes => new[] { "MY_EVENT" };

    public void HandleEvent(ISimulationContext context, Event evt)
    {
        var payload = evt.GetPayload<MyPayload>();
        // Handle the event...
    }
}
```

### Creating Typed Payloads

```csharp
public sealed record MyPayload(int Value, string Description) : IEventPayload;
```
