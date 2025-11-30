using Eventium.Core.Time;

namespace Eventium.Core.Tests.Time;

public class TimeModelTests
{

    [Fact]
    public void Constructor_ContinuousMode_SetsProperties()
    {
        var model = new TimeModel(TimeMode.Continuous);

        Assert.Equal(TimeMode.Continuous, model.Mode);
        Assert.Equal(1.0, model.Step); // default step
    }
    [Fact]
    public void Constructor_DiscreteMode_SetsProperties()
    {
        var model = new TimeModel(TimeMode.Discrete, step: 2.5);

        Assert.Equal(TimeMode.Discrete, model.Mode);
        Assert.Equal(2.5, model.Step);
    }

    [Fact]
    public void InitialTime_ReturnsZero()
    {
        Assert.Equal(0.0, TimeModel.InitialTime);
    }

    [Fact]
    public void NextStepTime_ContinuousMode_ThrowsInvalidOperationException()
    {
        var model = new TimeModel(TimeMode.Continuous);

        Assert.Throws<InvalidOperationException>(() => model.NextStepTime(1.0));
    }

    [Fact]
    public void NextStepTime_DiscreteMode_ReturnsCurrentPlusStep()
    {
        var model = new TimeModel(TimeMode.Discrete, step: 1.5);

        var next = model.NextStepTime(3.0);

        Assert.Equal(4.5, next);
    }

    [Fact]
    public void ToTurn_ContinuousMode_ThrowsInvalidOperationException()
    {
        var model = new TimeModel(TimeMode.Continuous);

        Assert.Throws<InvalidOperationException>(() => model.ToTurn(1.0));
    }

    [Fact]
    public void ToTurn_DiscreteMode_ReturnsCorrectTurnIndex()
    {
        var model = new TimeModel(TimeMode.Discrete, step: 1.0);

        Assert.Equal(0, model.ToTurn(0.0));
        Assert.Equal(1, model.ToTurn(1.0));
        Assert.Equal(5, model.ToTurn(5.0));
    }

    [Fact]
    public void ToTurn_DiscreteMode_WithCustomStep_ReturnsCorrectTurnIndex()
    {
        var model = new TimeModel(TimeMode.Discrete, step: 0.5);

        Assert.Equal(0, model.ToTurn(0.0));
        Assert.Equal(2, model.ToTurn(1.0));
        Assert.Equal(4, model.ToTurn(2.0));
    }

    [Fact]
    public void ToTurn_RoundsToNearestTurn()
    {
        var model = new TimeModel(TimeMode.Discrete, step: 1.0);

        Assert.Equal(1, model.ToTurn(0.9));
        Assert.Equal(1, model.ToTurn(1.1));
        Assert.Equal(2, model.ToTurn(1.6));
    }
}
