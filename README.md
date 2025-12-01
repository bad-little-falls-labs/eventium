# Eventium

[![codecov](https://codecov.io/gh/bad-little-falls-labs/eventium/branch/main/graph/badge.svg)](https://codecov.io/gh/bad-little-falls-labs/eventium)

Eventium is a modern, extensible event-driven simulation engine designed for modeling real-world systems, processes, and scenarios. It supports both turn-based (discrete) and continuous-time (timed) simulations through a unified scheduling and event-processing architecture.

## Features

-   **Dual Time Models**: Discrete (turn-based) and Continuous (real-time) simulation modes
-   **Entity-Component System**: Flexible ECS-style architecture for modeling complex entities
-   **Typed Event Payloads**: Type-safe event data with compile-time checking
-   **Extensible Systems**: Plug-in architecture for custom simulation logic
-   **Built-in Instrumentation**: Event logging and metrics collection

## Requirements

-   [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/bad-little-falls-labs/eventium.git
cd eventium
```

### Build the Solution

```bash
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Run the Sample Scenarios

```bash
dotnet run --project src/Eventium.Scenarios
```

## Project Structure

```text
eventium/
├── src/
│   ├── Eventium.Core/          # Core simulation engine library
│   └── Eventium.Scenarios/     # Sample simulation scenarios
├── tests/
│   └── Eventium.Core.Tests/    # Unit tests
└── Eventium.sln
```

## Development

### Pre-commit Hooks

This project uses [Husky.Net](https://github.com/alirezanet/Husky.Net) for Git pre-commit hooks. The hooks automatically run on each commit to ensure code quality:

1. **Format Check**: Validates code style with `dotnet format`
2. **Build**: Ensures the solution compiles without errors
3. **Test**: Runs all unit tests

#### First-time Setup

After cloning, restore the local tools to enable pre-commit hooks:

```bash
dotnet tool restore
dotnet husky install
```

#### Manual Hook Execution

To manually run the pre-commit checks:

```bash
dotnet husky run
```

### Code Style

Code style is enforced via `.editorconfig`. The pre-commit hooks will automatically check formatting. To auto-fix formatting issues:

```bash
dotnet format
```

## License

See [LICENSE](LICENSE) for details.
