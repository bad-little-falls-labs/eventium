using System.Diagnostics.CodeAnalysis;

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

    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Modifies instance state (Value)")]
    public void Increment(long amount = 1)
    {
        Value += amount;
    }
}
