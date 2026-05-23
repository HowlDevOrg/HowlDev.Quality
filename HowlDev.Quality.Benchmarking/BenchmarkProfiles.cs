using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace HowlDev.Quality.Benchmarking;

/// <summary>
/// A few default options to pass in as parameters. 
/// </summary>
public static class BenchmarkProfiles {
    public static ManualConfig ShortRun => ManualConfig.CreateMinimumViable().AddJob(Job.ShortRun);
    public static ManualConfig SilentShortRun => ManualConfig.CreateEmpty().AddJob(Job.ShortRun);
    public static ManualConfig MediumRun => ManualConfig.CreateMinimumViable().AddJob(Job.MediumRun);
    public static ManualConfig SilentMediumRun => ManualConfig.CreateEmpty().AddJob(Job.MediumRun);
    public static ManualConfig Longun => ManualConfig.CreateMinimumViable().AddJob(Job.LongRun);
    public static ManualConfig SilentLongRun => ManualConfig.CreateEmpty().AddJob(Job.LongRun);
    public static ManualConfig CI => SilentShortRun;
}

/*
var config = ManualConfig.CreateMinimumViable()
                .WithOptions(ConfigOptions.DisableLogFile);
*/
