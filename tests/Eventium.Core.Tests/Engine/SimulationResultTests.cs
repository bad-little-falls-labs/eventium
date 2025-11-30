namespace Eventium.Core.Tests.Engine;

public class SimulationResultTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var duration = TimeSpan.FromMilliseconds(100);

        var result = new SimulationResult(
            stopReason: SimulationStopReason.TimeReached,
            finalTime: 50.0,
            eventsProcessed: 100,
            eventsRemaining: 5,
            wallClockDuration: duration,
            entityCount: 10);

        Assert.Equal(SimulationStopReason.TimeReached, result.StopReason);
        Assert.Equal(50.0, result.FinalTime);
        Assert.Equal(100, result.EventsProcessed);
        Assert.Equal(5, result.EventsRemaining);
        Assert.Equal(duration, result.WallClockDuration);
        Assert.Equal(10, result.EntityCount);
    }

    [Fact]
    public void EventsPerSecond_CalculatesCorrectly()
    {
        var result = new SimulationResult(
            stopReason: SimulationStopReason.QueueEmpty,
            finalTime: 10.0,
            eventsProcessed: 1000,
            eventsRemaining: 0,
            wallClockDuration: TimeSpan.FromSeconds(2),
            entityCount: 0);

        Assert.Equal(500.0, result.EventsPerSecond);
    }

    [Fact]
    public void EventsPerSecond_ZeroDuration_ReturnsZero()
    {
        var result = new SimulationResult(
            stopReason: SimulationStopReason.QueueEmpty,
            finalTime: 0.0,
            eventsProcessed: 0,
            eventsRemaining: 0,
            wallClockDuration: TimeSpan.Zero,
            entityCount: 0);

        Assert.Equal(0.0, result.EventsPerSecond);
    }

    [Theory]
    [InlineData(SimulationStopReason.QueueEmpty)]
    [InlineData(SimulationStopReason.TimeReached)]
    [InlineData(SimulationStopReason.MaxEventsReached)]
    [InlineData(SimulationStopReason.StoppedByUser)]
    public void StopReason_AllValuesSupported(SimulationStopReason reason)
    {
        var result = new SimulationResult(
            stopReason: reason,
            finalTime: 0,
            eventsProcessed: 0,
            eventsRemaining: 0,
            wallClockDuration: TimeSpan.Zero,
            entityCount: 0);

        Assert.Equal(reason, result.StopReason);
    }

    [Fact]
    public void ToString_ContainsAllValues()
    {
        var result = new SimulationResult(
            stopReason: SimulationStopReason.MaxEventsReached,
            finalTime: 25.5,
            eventsProcessed: 50,
            eventsRemaining: 10,
            wallClockDuration: TimeSpan.FromMilliseconds(150),
            entityCount: 5);

        var str = result.ToString();

        Assert.Contains("MaxEventsReached", str);
        Assert.Contains("25.50", str);
        Assert.Contains("50", str);
        Assert.Contains("10", str);
        Assert.Contains("5", str);
    }
}
