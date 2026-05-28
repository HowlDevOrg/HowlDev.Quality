using System.ComponentModel;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using HowlDev.Quality.Benchmarking.Validators;

namespace HowlDev.Quality.Benchmarking;

/// <summary>
/// Use the <see cref="BenchmarkValidator&lt;T&gt;.WithProfile(ManualConfig)"/> function 
/// to create this type. 
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class BenchmarkValidatorWithConfig<T> : IBenchmarkValidator {
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

    /// <summary>
    /// Removes the log file created at the end of each run. This does not interfere
    /// with the DisassemblyOutput or Exporter functions, you can still get 
    /// those results. 
    /// </summary>
    public BenchmarkValidatorWithConfig<T> WithoutLogOutput() {
        config.WithOptions(ConfigOptions.DisableLogFile);
        return this;
    }

    /// <summary>
    /// Adds the [MemoryDiagnoser] attribute to the class. 
    /// </summary>
    public BenchmarkValidatorWithConfig<T> WithMemoryDiagnoser() {
        config.AddDiagnoser(MemoryDiagnoser.Default);
        return this;
    }

    /// <summary>
    /// Adds the [DisassemblyDiagnoser] attribute to the class. 
    /// </summary>
    public BenchmarkValidatorWithConfig<T> WithDisassemblyOutput() {
        config.AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig()));
        return this;
    }

    /// <summary>
    /// Adds the Github Markdown exporter to the config.
    /// </summary>
    public BenchmarkValidatorWithConfig<T> WithGithubExporter() {
        config.AddExporter(MarkdownExporter.GitHub);
        return this;
    }

    /// <summary>
    /// Adds the desired exporter to the config. 
    /// </summary>
    public BenchmarkValidatorWithConfig<T> WithExporter(IExporter exp) {
        config.AddExporter(exp);
        return this;
    }

    /// <inheritdoc/>
    public Summary Run(bool pauseOnInvalid = true) {
        HelperFunctions.ValidateMethods<T>(pauseOnInvalid, [.. actions.Keys]);

        Summary result = BenchmarkRunner.Run<T>(config);
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
        Summary result = BenchmarkRunner.Run<T>(config);
        return HelperFunctions.GetExceptions(result, actions);
    }
}
