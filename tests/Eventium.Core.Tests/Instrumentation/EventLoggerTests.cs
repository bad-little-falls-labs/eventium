// <copyright file="EventLoggerTests.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using Eventium.Core;
using Eventium.Core.Events;
using Eventium.Core.Instrumentation;
using Eventium.Core.Time;

namespace Eventium.Core.Tests.Instrumentation;

public class EventLoggerTests
{
    private static readonly EventHandlerDelegate DummyHandler = (_, _) => { };

    [Fact]
    public void Log_DoesNotThrow()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var evt = CreateEvent(1.0, "TEST_EVENT");

        // EventLogger is a static utility - just verify it doesn't throw
        var exception = Record.Exception(() => EventLogger.Log(engine, evt));

        Assert.Null(exception);
    }

    [Fact]
    public void Log_WithEmptyPayload_DoesNotThrow()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var evt = CreateEvent(1.0, "EMPTY_EVENT");

        var exception = Record.Exception(() => EventLogger.Log(engine, evt));

        Assert.Null(exception);
    }

    [Fact]
    public void Log_WithPayload_DoesNotThrow()
    {
        var engine = new SimulationEngine(new TimeModel(TimeMode.Continuous));
        var payload = new Dictionary<string, object?>
        {
            ["key1"] = "value1",
            ["key2"] = 42
        };
        var evt = new Event(1.0, 0, "TEST", payload, DummyHandler);

        var exception = Record.Exception(() => EventLogger.Log(engine, evt));

        Assert.Null(exception);
    }

    private static Event CreateEvent(double time, string type, int priority = 0)
    {
        return new Event(time, priority, type, (IDictionary<string, object?>?)null, DummyHandler);
    }
}
