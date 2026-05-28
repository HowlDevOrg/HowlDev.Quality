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
