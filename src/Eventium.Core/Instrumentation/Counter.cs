namespace Eventium.Core.Instrumentation;

/// <summary>
/// Simple monotonically increasing counter.
/// </summary>
public sealed class Counter
{
    public string Name { get; }
    public long Value { get; private set; }

    public Counter(string name)
    {
        Name = name;
    }

    public void Increment(long amount = 1)
    {
        Value += amount;
    }
}
