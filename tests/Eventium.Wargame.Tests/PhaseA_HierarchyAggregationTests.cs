// <copyright file="PhaseA_HierarchyAggregationTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

using Eventium.Core;
using Eventium.Core.Time;
using Eventium.Core.World;
using Eventium.Wargame.Components;
using Eventium.Wargame.Domain;
using Eventium.Wargame.Services;
using Eventium.Wargame.Systems;

namespace Eventium.Wargame.Tests;

/// <summary>
/// Phase A tests: Hierarchy construction, parent-child relationships, and aggregation.
/// </summary>
public sealed class PhaseA_HierarchyAggregationTests
{
    [Fact]
    public void CanCreateBasicHierarchy()
    {
        // Arrange
        var engine = new SimulationEngine(
            new TimeModel(TimeMode.Discrete, step: 1.0),
            seed: 42);

        // Create battalion
        var battalion = new Entity(id: 1, type: "FORMATION");
        battalion.AddComponent("ECHELON", new Echelon
        {
            Level = EchelonLevel.Battalion,
            Faction = "Blue",
            Designation = "1st Battalion"
        });
        battalion.AddComponent("HEALTH", new Health { MaxStrength = 400, CurrentStrength = 400 });
        battalion.AddComponent("CHILDREN", new Children());

        engine.World.AddEntity(battalion);

        // Create company as child
        var company = new Entity(id: 2, type: "FORMATION");
        company.AddComponent("ECHELON", new Echelon
        {
            Level = EchelonLevel.Company,
            Faction = "Blue",
            Designation = "A Company"
        });
        company.AddComponent("HEALTH", new Health { MaxStrength = 200, CurrentStrength = 200 });
        company.AddComponent("PARENT", new Parent { ParentEntityId = 1, IsActive = true });
        company.AddComponent("CHILDREN", new Children());

        engine.World.AddEntity(company);

        // Link them
        var battalionChildren = battalion.GetComponent<Children>("CHILDREN") as Children;
        if (battalionChildren != null)
        {
            battalionChildren.ChildEntityIds.Add(2);
        }

        // Assert
        Assert.NotNull(battalion.GetComponent<Echelon>("ECHELON"));
        Assert.NotNull(company.GetComponent<Parent>("PARENT"));
        Assert.Single(battalionChildren!.ChildEntityIds);
    }

    [Fact]
    public void FormationQueryService_ComputesTotalStrength()
    {
        // Arrange
        var engine = new SimulationEngine(
            new TimeModel(TimeMode.Discrete, step: 1.0),
            seed: 42);

        var battalion = new Entity(id: 1, type: "FORMATION");
        battalion.AddComponent("HEALTH", new Health { MaxStrength = 400, CurrentStrength = 400 });
        battalion.AddComponent("CHILDREN", new Children());
        engine.World.AddEntity(battalion);

        var company = new Entity(id: 2, type: "FORMATION");
        company.AddComponent("HEALTH", new Health { MaxStrength = 200, CurrentStrength = 200 });
        company.AddComponent("PARENT", new Parent { ParentEntityId = 1, IsActive = true });
        company.AddComponent("CHILDREN", new Children());
        engine.World.AddEntity(company);

        var platoon = new Entity(id: 3, type: "FORMATION");
        platoon.AddComponent("HEALTH", new Health { MaxStrength = 100, CurrentStrength = 100 });
        platoon.AddComponent("PARENT", new Parent { ParentEntityId = 2, IsActive = true });
        platoon.AddComponent("CHILDREN", new Children());
        engine.World.AddEntity(platoon);

        var battalionChildren = battalion.GetComponent<Children>("CHILDREN") as Children;
        if (battalionChildren != null)
        {
            battalionChildren.ChildEntityIds.Add(2);
        }

        var companyChildren = company.GetComponent<Children>("CHILDREN") as Children;
        if (companyChildren != null)
        {
            companyChildren.ChildEntityIds.Add(3);
        }

        var service = new FormationQueryService(engine.World);

        // Act
        int totalStrength = service.GetTotalStrength(1);

        // Assert
        // Battalion (400) + Company (200) + Platoon (100) = 700
        Assert.Equal(700, totalStrength);
    }

    [Fact]
    public void FormationQueryService_GetUnitsAtEchelon()
    {
        // Arrange
        var engine = new SimulationEngine(
            new TimeModel(TimeMode.Discrete, step: 1.0),
            seed: 42);

        // Create mixed units
        for (int i = 1; i <= 3; i++)
        {
            var unit = new Entity(id: i, type: "FORMATION");
            unit.AddComponent("ECHELON", new Echelon
            {
                Level = i == 1 ? EchelonLevel.Battalion : EchelonLevel.Company,
                Faction = "Blue",
                Designation = $"Unit {i}"
            });
            engine.World.AddEntity(unit);
        }

        var service = new FormationQueryService(engine.World);

        // Act
        var companies = service.GetUnitsAtEchelon(EchelonLevel.Company).ToList();
        var battalions = service.GetUnitsAtEchelon(EchelonLevel.Battalion).ToList();

        // Assert
        Assert.Equal(2, companies.Count);
        Assert.Single(battalions);
    }

    [Fact]
    public void FormationQueryService_ReturnsCorrectChildren()
    {
        // Arrange
        var engine = new SimulationEngine(
            new TimeModel(TimeMode.Discrete, step: 1.0),
            seed: 42);

        var battalion = new Entity(id: 1, type: "FORMATION");
        battalion.AddComponent("CHILDREN", new Children());
        engine.World.AddEntity(battalion);

        // Add 3 companies
        for (int i = 2; i <= 4; i++)
        {
            var company = new Entity(id: i, type: "FORMATION");
            company.AddComponent("PARENT", new Parent { ParentEntityId = 1, IsActive = true });
            company.AddComponent("CHILDREN", new Children());
            engine.World.AddEntity(company);
            var battalionChildren = battalion.GetComponent<Children>("CHILDREN") as Children;
            if (battalionChildren != null)
            {
                battalionChildren.ChildEntityIds.Add(i);
            }
        }

        var service = new FormationQueryService(engine.World);

        // Act
        var children = service.GetChildren(1).ToList();

        // Assert
        Assert.Equal(3, children.Count);
    }
}
