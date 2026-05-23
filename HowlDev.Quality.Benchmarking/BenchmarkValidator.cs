using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
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

    /// <summary>
    /// Pass in a default configuration. This will override the Config option 
    /// in the BenchmarkRunner process. <br/>
    /// Use the BenchmarkProfiles static class to select a config. 
    /// </summary>
    public BenchmarkValidatorWithConfig<T> WithProfile(ManualConfig config) {
        return new BenchmarkValidatorWithConfig<T>(actions, config);
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
    public Summary Run(bool pauseOnInvalid = true, ManualConfig? config = null) {
        HelperFunctions.ValidateMethods<T>(pauseOnInvalid, [.. actions.Keys]);

        Summary result = BenchmarkRunner.Run<T>(config);
        HelperFunctions.WriteBreaker();
        HelperFunctions.DisplayAndThrowErrors(result, actions);

        return result;
    }
}

/// <summary>
/// Hidden.
/// </summary>
public class BenchmarkValidatorWithConfig<T> {
    internal BenchmarkValidatorWithConfig(Dictionary<string, BenchmarkExpectations> act, ManualConfig c) {
        actions = act;
        config = c;
    }
    private ManualConfig config;
    private Dictionary<string, BenchmarkExpectations> actions = [];
    /// <summary>
    /// Provide the method name for a <c>[Benchmark]</c>ed function. Then use the fluent 
    /// builder for an Expectation. <br/>
    /// Any methods not provided (for Bytes or Nanoseconds) will default to null and 
    /// will not be tested (will always pass no matter their result). 
    /// </summary>
    public BenchmarkValidatorWithConfig<T> Expect(string methodName, BenchmarkExpectations exp) {
        actions.Add(methodName, exp);
        return this;
    }

    public BenchmarkValidatorWithConfig<T> WithoutLogOutput() {
        config.WithOptions(ConfigOptions.DisableLogFile);
        return this;
    }

    public BenchmarkValidatorWithConfig<T> WithMemoryDiagnoser() {
        config.AddDiagnoser(MemoryDiagnoser.Default);
        return this;
    }

    public BenchmarkValidatorWithConfig<T> WithDisassemblyOutput() {
        config.AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig()));
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
    public Summary Run(bool pauseOnInvalid = true) {
        HelperFunctions.ValidateMethods<T>(pauseOnInvalid, [.. actions.Keys]);

        Summary result = BenchmarkRunner.Run<T>(config);
        HelperFunctions.WriteBreaker();
        HelperFunctions.DisplayAndThrowErrors(result, actions);

        return result;
    }
}
