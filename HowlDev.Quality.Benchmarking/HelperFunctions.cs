using System.Reflection;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Reports;

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
        WriteBreaker();
    }

    internal static void DisplayAndThrowErrors(Summary result, Dictionary<string, BenchmarkExpectations> actions) {
        List<BenchmarkException> exceptions = [];
        foreach (BenchmarkReport report in result.Reports) {
            string methodName = report.BenchmarkCase.Descriptor.WorkloadMethod.Name;
            try {
                if (actions.TryGetValue(methodName, out BenchmarkExpectations? exp)) {
                    exp.Report(report);
                }
            } catch (Exception ex) {
                if (ex is BenchmarkException ex1) {
                    exceptions.Add(ex1);
                } else if (ex is InvalidDataException) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Somehow got an InvalidDataException in method {methodName}.");
                } else {
                    throw;
                }
            }
        }

        if (exceptions.Count > 0) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Exceptions thrown: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (BenchmarkException exception in exceptions) {
                Console.WriteLine("- " + exception.Message);
            }

            Console.ForegroundColor = ConsoleColor.White;
            WriteBreaker();
            Console.ForegroundColor = ConsoleColor.Red;
            throw new AggregateException("Exceptions were thrown. Scroll up to see the results.", exceptions);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBreaker() {
        Console.WriteLine("================");
    }
}
