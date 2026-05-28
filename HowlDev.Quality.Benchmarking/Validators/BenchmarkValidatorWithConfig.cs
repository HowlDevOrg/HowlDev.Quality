using System.ComponentModel;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace HowlDev.Quality.Benchmarking.Validators;

/// <summary>
/// Use the <see cref="BenchmarkValidatorBase&lt;T&gt;.WithProfile(ManualConfig)"/> function 
/// to create this type. 
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class BenchmarkValidatorWithConfig<T> : BenchmarkValidatorAbstractBase<T> {
    internal BenchmarkValidatorWithConfig(ManualConfig c) {
        actions = [];
        config = c;
    }
    private ManualConfig config;

    /// <inheritdoc/>
    public override BenchmarkValidatorWithConfig<T> Expect(string methodName, BenchmarkExpectations exp) {
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
    protected override Summary RunBenchmark() {
        return BenchmarkRunner.Run<T>(config);
    }
}

/// <summary>
/// Use the <see cref="BenchmarkValidatorBase&lt;T&gt;.WithProfile(ManualConfig)"/> function 
/// to create this type. 
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public class BenchmarkValidatorWithConfig<T, P> : IBenchmarkValidator {
    internal BenchmarkValidatorWithConfig(ManualConfig c, string fieldName) {
        actions = [];
        config = c;
        name = fieldName;
    }
    private Dictionary<string, List<(P, BenchmarkExpectations)>> actions = [];
    private ManualConfig config;
    private string name;

    /// <summary>
    /// Add a new expectation for a given Param value and BenchmarkExpectation object.
    /// </summary>
    /// <param name="methodName">Method name</param>
    /// <param name="paramValue">Value of param</param>
    /// <param name="exp">Expectation</param>
    public BenchmarkValidatorWithConfig<T, P> Expect(string methodName, P paramValue, BenchmarkExpectations exp) {
        if (actions.TryGetValue(methodName, out var list)) {
            list.Add((paramValue, exp));
        } else {
            actions.Add(methodName, [(paramValue, exp)]);
        }

        return this;
    }

    /// <summary>
    /// Removes the log file created at the end of each run. This does not interfere
    /// with the DisassemblyOutput or Exporter functions, you can still get 
    /// those results. 
    /// </summary>
    public BenchmarkValidatorWithConfig<T, P> WithoutLogOutput() {
        config.WithOptions(ConfigOptions.DisableLogFile);
        return this;
    }

    /// <summary>
    /// Adds the [MemoryDiagnoser] attribute to the class. 
    /// </summary>
    public BenchmarkValidatorWithConfig<T, P> WithMemoryDiagnoser() {
        config.AddDiagnoser(MemoryDiagnoser.Default);
        return this;
    }

    /// <summary>
    /// Adds the [DisassemblyDiagnoser] attribute to the class. 
    /// </summary>
    public BenchmarkValidatorWithConfig<T, P> WithDisassemblyOutput() {
        config.AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig()));
        return this;
    }

    /// <summary>
    /// Adds the Github Markdown exporter to the config.
    /// </summary>
    public BenchmarkValidatorWithConfig<T, P> WithGithubExporter() {
        config.AddExporter(MarkdownExporter.GitHub);
        return this;
    }

    /// <summary>
    /// Adds the desired exporter to the config. 
    /// </summary>
    public BenchmarkValidatorWithConfig<T, P> WithExporter(IExporter exp) {
        config.AddExporter(exp);
        return this;
    }

    /// <inheritdoc/>
    public Summary Run(bool pauseOnInvalid = true) {
        Validate(pauseOnInvalid);

        Summary result = BenchmarkRunner.Run<T>(config);
        HelperFunctions.WriteBreaker();
        HelperFunctions.DisplayAndThrowErrors(result, actions, typeof(T).Name);

        return result;
    }

    /// <inheritdoc/>
    public List<BenchmarkException> RunAndCollectExceptions() {
        Summary result = BenchmarkRunner.Run<T>(config);
        return HelperFunctions.GetExceptions(result, actions, typeof(T).Name);
    }

    /// <inheritdoc/>
    public void Validate(bool pauseOnInvalid = true) {
        HelperFunctions.ValidateMethods<T>(pauseOnInvalid, [.. actions.Keys]);
        HelperFunctions.ValidateParams<T, P>(pauseOnInvalid, actions, name);
    }
}
