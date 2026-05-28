using System.ComponentModel;
using BenchmarkDotNet.Configs;

namespace HowlDev.Quality.Benchmarking.Validators;

/// <summary>
/// Use the static function call on BenchmarkValidator. <br/>
/// Allows the branch for Expect calls (no config), Profile calls
/// (for configurations), and Param calls (for Param-based benchmarks).
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class BenchmarkValidatorBase<T> {
    internal BenchmarkValidatorBase() {}

    /// <summary>
    /// Returns a non-configurable <c>BenchmarkValidator&lt;T&gt;</c> with 
    /// as many methods as needed. 
    /// </summary>
    public BenchmarkValidator<T> Expect(string methodName, BenchmarkExpectations exp) {
        return new BenchmarkValidator<T>().Expect(methodName, exp);
    }

    /// <summary>
    /// Pass in a default configuration. This will override the Config option 
    /// in the BenchmarkRunner process. <br/>
    /// Use the <c>BenchmarkProfiles</c> static class to select a config. <br/>
    /// <remarks>NOTE: This may interfere with any attributes left on the class, so it's 
    /// recommended to remove all of them and do them through these functions.
    /// You should still have [Benchmark] on the functions.</remarks>
    /// </summary>
    public BenchmarkValidatorWithConfig<T> WithProfile(ManualConfig config) {
        return new BenchmarkValidatorWithConfig<T>([], config);
    }
}
