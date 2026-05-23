using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;

namespace HowlDev.Quality.Benchmarking;

/// <summary>
/// A few default options to pass in as parameters. 
/// </summary>
public static class BenchmarkProfiles {
    public static ManualConfig ShortRun => ManualConfig.CreateMinimumViable().AddJob(Job.ShortRun);
    public static ManualConfig ShortRunWithMemory => ManualConfig.CreateMinimumViable().AddJob(Job.ShortRun).AddDiagnoser(MemoryDiagnoser.Default);
    public static ManualConfig SilentShortRun => ManualConfig.CreateEmpty().AddJob(Job.ShortRun);
    public static ManualConfig SilentShortRunWithMemory => ManualConfig.CreateEmpty().AddJob(Job.ShortRun).AddDiagnoser(MemoryDiagnoser.Default);
}

/*
var config = ManualConfig.CreateMinimumViable()
                .WithOptions(ConfigOptions.DisableLogFile);
*/
