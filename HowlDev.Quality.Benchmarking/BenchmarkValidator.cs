using System.Runtime.CompilerServices;
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
        HelperFunctions.ValidateMethods<T>(pauseOnInvalid, [..actions.Keys]);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteBreaker() {
        Console.WriteLine("================");
    }
}
