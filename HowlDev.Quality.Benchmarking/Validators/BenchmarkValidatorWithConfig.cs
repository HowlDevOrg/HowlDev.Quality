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
    internal BenchmarkValidatorWithConfig(Dictionary<string, BenchmarkExpectations> act, ManualConfig c) {
        actions = act;
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
