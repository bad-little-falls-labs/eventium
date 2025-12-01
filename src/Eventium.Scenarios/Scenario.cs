// <copyright file="Scenario.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core;
using Eventium.Core.Events;

namespace Eventium.Scenarios;

/// <summary>
/// Bundles world initialization and initial events.
/// </summary>
public delegate void ScenarioInitializer(SimulationEngine engine);

public sealed class Scenario
{

    public Scenario(
        string name,
        string description,
        ScenarioInitializer? initializer,
        IEnumerable<Event>? initialEvents)
    {
        Name = name;
        Description = description;
        Initializer = initializer;
        InitialEvents = initialEvents is null
            ? Array.Empty<Event>()
            : new List<Event>(initialEvents);
    }
    public string Description { get; }
    public IReadOnlyCollection<Event> InitialEvents { get; }
    public ScenarioInitializer? Initializer { get; }
    public string Name { get; }

    public void Apply(SimulationEngine engine)
    {
        Initializer?.Invoke(engine);
        foreach (var evt in InitialEvents)
        {
            engine.Queue.Enqueue(evt);
        }
    }
}
