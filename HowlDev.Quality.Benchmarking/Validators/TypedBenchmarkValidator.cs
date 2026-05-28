using System.ComponentModel;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace HowlDev.Quality.Benchmarking.Validators; 

/// <summary>
/// Use the static function call on BenchmarkValidatorBase.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class BenchmarkValidator<T> : IBenchmarkValidator {
    internal BenchmarkValidator() { }
    private Dictionary<string, BenchmarkExpectations> actions = [];
    /// <summary>
    /// Provide the method name for a <c>[Benchmark]</c>ed function. Then use the fluent 
    /// builder for an Expectation. <br/>
    /// Any methods not provided (for Bytes or Nanoseconds) will default to null and 
    /// will not be tested (will always pass no matter their result). 
    /// </summary>
    public BenchmarkValidator<T> Expect(string methodName, BenchmarkExpectations exp) {
        actions.Add(methodName, exp);
        return this;
    }

    /// <inheritdoc/>
    public Summary Run(bool pauseOnInvalid = true) {
        HelperFunctions.ValidateMethods<T>(pauseOnInvalid, [.. actions.Keys]);

        Summary result = BenchmarkRunner.Run<T>();
        HelperFunctions.WriteBreaker();
        HelperFunctions.DisplayAndThrowErrors(result, actions);

        return result;
    }

    /// <inheritdoc/>
    public void Validate(bool pauseOnInvalid = true) {
        HelperFunctions.ValidateMethods<T>(pauseOnInvalid, [.. actions.Keys]);
    }

    /// <inheritdoc/>
    public List<BenchmarkException> RunAndCollectExceptions() {
        Summary result = BenchmarkRunner.Run<T>();
        return HelperFunctions.GetExceptions(result, actions);
    }
}
