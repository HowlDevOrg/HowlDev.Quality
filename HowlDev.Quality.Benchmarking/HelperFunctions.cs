using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace HowlDev.Quality.Benchmarking; 

internal static class HelperFunctions {
    public static void ValidateMethods<T>(bool pauseOnInvalid, List<string> providedMethods) {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Validating methods provided by you and by the type...");
        List<string> benchmarkedMethods = [.. typeof(T)
            .GetMethods()
            .Where(a => a.GetCustomAttribute<BenchmarkAttribute>() != null)
            .Select(a => a.Name)];

        bool invalid = false;

        IEnumerable<string> uncheckedBenchmarks = benchmarkedMethods.Where(a => !providedMethods.Contains(a));
        IEnumerable<string> unusedProvided = providedMethods.Where(a => !benchmarkedMethods.Contains(a));

        ConsoleColor problemColor = ConsoleColor.Yellow;

        if (uncheckedBenchmarks.Any()) {
            Console.WriteLine("Benchmarks without a validator: ");
            Console.ForegroundColor = problemColor;
            foreach (string item in uncheckedBenchmarks) {
                Console.WriteLine("- " + item);
            }

            invalid = true;
        }

        if (unusedProvided.Any()) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Provided methods that don't tie into a benchmarked method: ");
            Console.ForegroundColor = problemColor;
            foreach (string item in unusedProvided) {
                Console.WriteLine("- " + item);
            }

            invalid = true;
        }

        if (pauseOnInvalid && invalid) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Paused here. Press any key to continue. Press Ctrl + C to stop.");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("To not pause on validation problems, pass False to the Run method.");
            Console.ReadKey(true);
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Validation complete! Starting benchmarks.");
        Console.WriteLine("============");
    }
}
