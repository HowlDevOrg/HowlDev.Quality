using System.ComponentModel;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace HowlDev.Quality.Benchmarking.Validators;

/// <summary>
/// Use the static function call on BenchmarkValidatorBase.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class BenchmarkValidator<T> : BenchmarkValidatorAbstractBase<T> {
    internal BenchmarkValidator() { }

    /// <inheritdoc/>
    public override BenchmarkValidator<T> Expect(string methodName, BenchmarkExpectations exp) {
        actions.Add(methodName, exp);
        return this;
    }

    /// <inheritdoc/>
    protected override Summary RunBenchmark() {
        return BenchmarkRunner.Run<T>();
    }
}

/// <summary>
/// Use the static function call on BenchmarkValidatorBase with two types.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class BenchmarkValidator<T, P> : IBenchmarkValidator {
    private Dictionary<string, List<(P, BenchmarkExpectations)>> actions = [];
    private string name;
    internal BenchmarkValidator(string paramName) { name = paramName; }

    /// <summary>
    /// Add a new expectation for a given Param value and BenchmarkExpectation object.
    /// </summary>
    /// <param name="methodName">Method name</param>
    /// <param name="paramValue">Value of param</param>
    /// <param name="exp">Expectation</param>
    public BenchmarkValidator<T, P> Expect(string methodName, P paramValue, BenchmarkExpectations exp) {
        if (actions.TryGetValue(methodName, out var list)) {
            list.Add((paramValue, exp));
        } else {
            actions.Add(methodName, [(paramValue, exp)]);
        }

        return this;
    }

    /// <inheritdoc/>
    public Summary Run(bool pauseOnInvalid = true) {
        Validate(pauseOnInvalid);

        Summary result = BenchmarkRunner.Run<T>();
        HelperFunctions.WriteBreaker();
        HelperFunctions.DisplayAndThrowErrors(result, actions, typeof(T).Name);

        return result;
    }

    /// <inheritdoc/>
    public List<BenchmarkException> RunAndCollectExceptions() {
        Summary result = BenchmarkRunner.Run<T>();
        return HelperFunctions.GetExceptions(result, actions, typeof(T).Name);
    }

    /// <inheritdoc/>
    public void Validate(bool pauseOnInvalid = true) {
        HelperFunctions.ValidateMethods<T>(pauseOnInvalid, [.. actions.Keys]);
        HelperFunctions.ValidateParams<T, P>(pauseOnInvalid, actions, name);
    }
}
