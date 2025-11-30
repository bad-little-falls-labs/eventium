using Eventium.Core.World;

namespace Eventium.Scenarios.SimpleDiscrete;

public sealed class PositionComponent : IComponent
{
    public int X { get; set; }
    public int Y { get; set; }
}
