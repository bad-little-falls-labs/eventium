// <copyright file="EventLogger.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using Eventium.Core.Events;

namespace Eventium.Core.Instrumentation;

/// <summary>
/// Minimal console-based event logger.
/// Can be registered as a handler in scenarios.
/// </summary>
public static class EventLogger
{
    public static void Log(SimulationEngine engine, Event evt)
    {
        Console.WriteLine($"[t={engine.Time:0.###}] {evt.Type} {FormatPayload(evt)}");
    }

    private static string FormatPayload(Event evt)
    {
        if (evt.Payload.Count == 0)
            return "{}";

        var parts = new List<string>();
        foreach (var kvp in evt.Payload)
        {
            parts.Add($"{kvp.Key}={kvp.Value ?? "null"}");
        }

        return "{ " + string.Join(", ", parts) + " }";
    }
}
