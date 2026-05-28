using System.ComponentModel;
using BenchmarkDotNet.Reports;

namespace HowlDev.Quality.Benchmarking.Validators;

/// <summary>
/// Abstract base class containing shared functionality for benchmark validators.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class BenchmarkValidatorAbstractBase<T> : IBenchmarkValidator {
    /// <summary/> 
    protected Dictionary<string, BenchmarkExpectations> actions = [];

    /// <summary>
    /// Provide the method name for a <c>[Benchmark]</c>ed function. Then use the fluent 
    /// builder for an Expectation. <br/>
    /// Any methods not provided (for Bytes or Nanoseconds) will default to null and 
    /// will not be tested (will always pass no matter their result). 
    /// </summary>
    public abstract BenchmarkValidatorAbstractBase<T> Expect(string methodName, BenchmarkExpectations exp);

    /// <summary>
    /// Runs the benchmark and validates expectations. Must be implemented by derived classes.
    /// </summary>
    protected abstract Summary RunBenchmark();

    /// <inheritdoc/>
    public Summary Run(bool pauseOnInvalid = true) {
        HelperFunctions.ValidateMethods<T>(pauseOnInvalid, [.. actions.Keys]);

        Summary result = RunBenchmark();
        HelperFunctions.WriteBreaker();
        HelperFunctions.DisplayAndThrowErrors(result, actions, typeof(T).Name);

        return result;
    }

    /// <inheritdoc/>
    public void Validate(bool pauseOnInvalid = true) {
        HelperFunctions.ValidateMethods<T>(pauseOnInvalid, [.. actions.Keys]);
    }

    /// <inheritdoc/>
    public List<BenchmarkException> RunAndCollectExceptions() {
        Summary result = RunBenchmark();
        return HelperFunctions.GetExceptions(result, actions, typeof(T).Name);
    }
}
