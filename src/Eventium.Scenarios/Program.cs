using System;
using Eventium.Scenarios.SimpleContinuous;
using Eventium.Scenarios.SimpleDiscrete;

namespace Eventium.Scenarios;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Eventium demo starting...");

        Console.WriteLine("\n=== Discrete Demo ===");
        SimpleDiscreteDemo.Run();

        Console.WriteLine("\n=== Continuous Demo ===");
        SimpleContinuousDemo.Run();
    }
}