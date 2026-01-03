// <copyright file="HierarchyScaleScenario.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core;
using Eventium.Core.Time;
using Eventium.Core.World;
using Eventium.Wargame.Components;
using Eventium.Wargame.Domain;
using Eventium.Wargame.Systems;

namespace Eventium.Wargame.Scenarios;

/// <summary>
/// Scenario that demonstrates a multi-level military hierarchy (squad → platoon → company → battalion).
/// Tests aggregation, parent-child relationships, and formation strength computation.
/// </summary>
public static class HierarchyScaleScenario
{
    /// <summary>
    /// Creates and runs the hierarchy scale scenario.
    /// </summary>
    public static SimulationResult Run()
    {
        var engine = new SimulationEngine(
            new TimeModel(TimeMode.Discrete, step: 1.0),
            seed: 42);

        // Register systems
        engine.RegisterSystem(new AggregationSystem());
        engine.RegisterSystem(new AggregationPulseSchedulerSystem(pulseInterval: 1.0));
        engine.RegisterSystem(new HierarchyValidationSystem());

        // Build hierarchy: Battalion → Company → Platoon → Squad
        BuildHierarchy(engine);

        // Schedule initial validation and aggregation
        engine.Schedule(
            time: 0.0,
            type: "VALIDATE_HIERARCHY",
            payload: new EmptyPayload());

        engine.Schedule(
            time: 1.0,
            type: "TURN_START",
            payload: new EmptyPayload());

        var result = engine.Run(untilTime: 10.0);
        return result;
    }

    /// <summary>
    /// Builds a three-level hierarchy for testing.
    /// </summary>
    private static void BuildHierarchy(SimulationEngine engine)
    {
        int entityCounter = 1;

        // Battalion (top level, 400 strength)
        var battalion = new Entity(
            id: entityCounter++,
            type: "FORMATION");
        battalion.AddComponent("ECHELON", new Echelon
        {
            Level = EchelonLevel.Battalion,
            Faction = "RedForce",
            Designation = "1st Battalion"
        });
        battalion.AddComponent("TRANSFORM2D", new Transform2D { X = 0, Y = 0 });
        battalion.AddComponent("HEALTH", new Health { MaxStrength = 400, CurrentStrength = 400, Armor = 10 });
        battalion.AddComponent("CHILDREN", new Children());
        battalion.AddComponent("FORMATION_AGGREGATE", new FormationAggregate { LastAggregationTime = 0.0 });

        int battalionId = battalion.Id;
        engine.World.AddEntity(battalion);

        // Create 2 Companies under the Battalion
        for (int c = 0; c < 2; c++)
        {
            var company = new Entity(id: entityCounter++, type: "FORMATION");
            company.AddComponent("ECHELON", new Echelon
            {
                Level = EchelonLevel.Company,
                Faction = "RedForce",
                Designation = $"Company {(char)('A' + c)}"
            });
            company.AddComponent("TRANSFORM2D", new Transform2D { X = c * 50, Y = 0 });
            company.AddComponent("HEALTH", new Health { MaxStrength = 200, CurrentStrength = 200, Armor = 10 });
            company.AddComponent("PARENT", new Parent { ParentEntityId = battalionId, IsActive = true });
            company.AddComponent("CHILDREN", new Children());
            company.AddComponent("FORMATION_AGGREGATE", new FormationAggregate { LastAggregationTime = 0.0 });

            int companyId = company.Id;
            var battalionChildren = battalion.GetComponent<Children>("CHILDREN") as Children;
            if (battalionChildren != null)
            {
                battalionChildren.ChildEntityIds.Add(companyId);
            }
            engine.World.AddEntity(company);

            // Create 3 Platoons under each Company
            for (int p = 0; p < 3; p++)
            {
                var platoon = new Entity(id: entityCounter++, type: "FORMATION");
                platoon.AddComponent("ECHELON", new Echelon
                {
                    Level = EchelonLevel.Platoon,
                    Faction = "RedForce",
                    Designation = $"Platoon {(char)('A' + p)}"
                });
                platoon.AddComponent("TRANSFORM2D", new Transform2D { X = c * 50 + p * 15, Y = 20 });
                platoon.AddComponent("HEALTH", new Health { MaxStrength = 30, CurrentStrength = 30, Armor = 5 });
                platoon.AddComponent("PARENT", new Parent { ParentEntityId = companyId, IsActive = true });
                platoon.AddComponent("CHILDREN", new Children());
                platoon.AddComponent("FORMATION_AGGREGATE", new FormationAggregate { LastAggregationTime = 0.0 });

                int platoonId = platoon.Id;
                var companyChildren = company.GetComponent<Children>("CHILDREN") as Children;
                if (companyChildren != null)
                {
                    companyChildren.ChildEntityIds.Add(platoonId);
                }
                engine.World.AddEntity(platoon);

                // Create 2 Squads under each Platoon
                for (int s = 0; s < 2; s++)
                {
                    var squad = new Entity(id: entityCounter++, type: "FORMATION");
                    squad.AddComponent("ECHELON", new Echelon
                    {
                        Level = EchelonLevel.Squad,
                        Faction = "RedForce",
                        Designation = $"Squad {(char)('1' + s)}"
                    });
                    squad.AddComponent("TRANSFORM2D", new Transform2D { X = c * 50 + p * 15 + s * 5, Y = 35 });
                    squad.AddComponent("HEALTH", new Health { MaxStrength = 10, CurrentStrength = 10, Armor = 0 });
                    squad.AddComponent("PARENT", new Parent { ParentEntityId = platoonId, IsActive = true });
                    squad.AddComponent("CHILDREN", new Children());
                    squad.AddComponent("FORMATION_AGGREGATE", new FormationAggregate { LastAggregationTime = 0.0 });

                    int squadId = squad.Id;
                    var platoonChildren = platoon.GetComponent<Children>("CHILDREN") as Children;
                    if (platoonChildren != null)
                    {
                        platoonChildren.ChildEntityIds.Add(squadId);
                    }
                    engine.World.AddEntity(squad);
                }
            }
        }
    }
}

/// <summary>
/// Empty payload for events that don't carry data.
/// </summary>
public sealed record EmptyPayload : Eventium.Core.Events.IEventPayload;
