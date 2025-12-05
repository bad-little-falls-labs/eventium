# Systems

Systems are plug-in handlers responding to specific event types.

## Implementing a System

- Implement `ISystem`
- Provide `HandledEventTypes`
- Implement `HandleEvent(ISimulationContext, Event)`

## Best Practices

- Keep side effects minimal
- Use typed payloads for safety
- Consider performance for hot paths
