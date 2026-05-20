using System.Reflection;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace HowlDev.Quality.Benchmarking;

/// <summary>
/// Base class for creating a benchmark validation suite. <br/>
/// 
/// This is done per-class and per method. Currently, there is only support for 
/// methods that run alone and don't depend on <c>[Params]</c> attributes. <br/>
/// 
/// An example is provided below. 
/// 
/// <code>
/// BenchmarkValidator.For&lt;SampleBenchmark&gt;()
///    .Expect("AdditionWithTimer", BenchmarkExpectations.ExpectedNanosecondsLT(10).WithBytes(0))
///    .Expect("AdditionWithMemory1", BenchmarkExpectations.ExpectedBytes(64))
///    .Expect("AdditionWithMemory2", BenchmarkExpectations.ExpectedBytes(7008).WithMicroseconds(0.25))
///    .Run();
/// </code>
/// </summary>
public static class BenchmarkValidator {
    /// <summary>
    /// Create a typed BenchmarkValidator. <br/>
    /// This should be the type of the class the you have 
    /// <c>[Benchmark]</c> attributes on. 
    /// </summary>
    public static BenchmarkValidator<T> For<T>() => new BenchmarkValidator<T>();
}

/// <summary>
/// Hidden.
/// </summary>
public class BenchmarkValidator<T> {
    internal BenchmarkValidator() {}
    private Dictionary<string, BenchmarkExpectations> actions = [];
    /// <summary>
    /// Provide the method name for a <c>[Benchmark]</c>ed function. Then use the fluent 
    /// builder for an Expectation. <br/>
    /// Any methods not provided (for Bytes or Nanoseconds) will default to null and 
    /// will not be tested (will always pass no matter their result). 
    /// </summary>
    /// <param name="methodName"></param>
    /// <param name="exp"></param>
    /// <returns></returns>
    public BenchmarkValidator<T> Expect(string methodName, BenchmarkExpectations exp) {
        actions.Add(methodName, exp);
        return this;
    }

    /// <summary>
    /// Takes the type and the provided expectations and matches them together. If 
    /// the parameter is set to true (default), then it will stop if there's a 
    /// mismatch of provided method names and benchmarked methods. This will happen before
    /// any benchmarks are run. <br/>
    /// Runs the default BenchmarkRunner system. Returns the <c>Summary</c> from 
    /// the result after running known tests. <br/>
    /// <hr/>
    /// To get the current results for a benchmark suite without displaying exceptions, 
    /// call the DebugDisplay extension on the result, as below: 
    /// <code>
    /// BenchmarkValidator.For&lt;SampleBenchmark&gt;()
    ///    .Run()
    ///    .DebugDisplay();
    /// </code>
    /// </summary>
    /// <param name="pauseOnInvalid"></param>
    /// <returns></returns>
    public Summary Run(bool pauseOnInvalid = true) {
        ValidateMethods(pauseOnInvalid);

        Summary result = BenchmarkRunner.Run<T>();
        WriteBreaker();
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

        return result;
    }

    private void ValidateMethods(bool pauseOnInvalid) {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Validating methods provided by you and by the type...");
        List<string> benchmarkedMethods = [.. typeof(T)
            .GetMethods()
            .Where(a => a.GetCustomAttribute<BenchmarkAttribute>() != null)
            .Select(a => a.Name)];

        List<string> providedMethods = [.. actions.Keys];
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteBreaker() {
        Console.WriteLine("================");
    }
}
