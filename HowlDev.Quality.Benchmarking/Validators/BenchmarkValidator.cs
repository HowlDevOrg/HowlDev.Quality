namespace HowlDev.Quality.Benchmarking.Validators;

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
    public static BenchmarkValidatorBase<T> For<T>() => new BenchmarkValidatorBase<T>();
}
