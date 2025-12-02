# Events & Payloads

Events are immutable records with time, type, payload, and handler.

## Typed Payloads

Use records implementing `IEventPayload` for immutability and type safety.

```csharp
public sealed record MyPayload(int Value, string Description) : IEventPayload;
```

## Scheduling

- Absolute time: `engine.Schedule(time: 5.0, type: "MY_EVENT", payload: new MyPayload(...))`
- Relative time: `engine.ScheduleIn(dt: 3.0, type: "DELAYED_EVENT")`
