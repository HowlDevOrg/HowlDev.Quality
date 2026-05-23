using BenchmarkDotNet.Reports;

namespace HowlDev.Quality.Benchmarking;

/// <summary>
/// Use the static builder.
/// </summary>
public interface IBenchmarkValidator<T> {
    /// <summary>
    /// Provide the method name for a <c>[Benchmark]</c>ed function. Then use the fluent 
    /// builder for an Expectation. <br/>
    /// Any methods not provided (for Bytes or Nanoseconds) will default to null and 
    /// will not be tested (will always pass no matter their result). 
    /// </summary>
    BenchmarkValidator<T> Expect(string methodName, BenchmarkExpectations exp);
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
    Summary Run(bool pauseOnInvalid = true);
}
