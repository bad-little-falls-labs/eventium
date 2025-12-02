# Getting Started

This guide helps you set up and run a simple Eventium simulation.

## Install

- .NET 9 SDK installed
- Clone repository and build: `dotnet build`

## Basic Simulation

- Create an `ISystem` implementation handling a custom event type.
- Register the system and schedule events via `SimulationEngine`.

See `src/Eventium.Scenarios` for runnable examples.
