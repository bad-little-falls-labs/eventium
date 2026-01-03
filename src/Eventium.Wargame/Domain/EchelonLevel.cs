// <copyright file="EchelonLevel.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>

namespace Eventium.Wargame.Domain;

/// <summary>
/// Represents the hierarchical level of a military formation/echelon.
/// </summary>
public enum EchelonLevel
{
    /// <summary>Individual soldier or basic unit element.</summary>
    Soldier = 0,

    /// <summary>Small group: 4-10 soldiers (fire team, section).</summary>
    Team = 1,

    /// <summary>Group of teams: 8-10 soldiers (rifle squad).</summary>
    Squad = 2,

    /// <summary>Multiple squads: 20-40 soldiers (platoon).</summary>
    Platoon = 3,

    /// <summary>Multiple platoons: 100-200 soldiers (company).</summary>
    Company = 4,

    /// <summary>Multiple companies: 300-800 soldiers (battalion).</summary>
    Battalion = 5,

    /// <summary>Multiple battalions: 2000-5000 soldiers (brigade).</summary>
    Brigade = 6,

    /// <summary>Multiple brigades: 10000-15000 soldiers (division).</summary>
    Division = 7,

    /// <summary>Multiple divisions: 40000-60000 soldiers (corps).</summary>
    Corps = 8,

    /// <summary>Multiple corps: 100000+ soldiers (army).</summary>
    Army = 9,
}
