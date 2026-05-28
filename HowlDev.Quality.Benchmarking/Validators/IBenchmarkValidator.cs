using BenchmarkDotNet.Reports;

namespace HowlDev.Quality.Benchmarking.Validators;

/// <summary>
/// Interface for the BenchmarkValidator suite. 
/// </summary>
public interface IBenchmarkValidator : IBenchmarkRunner, IGroupBenchmark { }

/// <summary>
/// Interface for the .Run method. 
/// </summary>
public interface IBenchmarkRunner {
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

/// <summary>
/// Interface for internal/debug methods for group benchmark 
/// runners. 
/// </summary>
public interface IGroupBenchmark {

    /// <summary>
    /// Run the MethodValidation tests to display any errors to the console. 
    /// Only intended for use in the group benchmark runner. 
    /// </summary>
    void Validate(bool pauseOnInvalid = true);

    /// <summary>
    /// Runs the benchmark and returns any benchmark exceptions. 
    /// Only intended for use in the group benchmark runner. 
    /// </summary>
    List<BenchmarkException> RunAndCollectExceptions();
}
