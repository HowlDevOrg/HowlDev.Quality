using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using HowlDev.Quality.Benchmarking.Validators;

namespace HowlDev.Quality.Benchmarking;

/// <summary>
/// Some default profiles to use in the <see cref="BenchmarkValidator&lt;T&gt;.WithProfile(ManualConfig)"/>
/// function. Provides each level of jobs with a basic and a Silent version. 
/// </summary>
public static class BenchmarkProfiles {
    /// <summary>
    /// Adds the [ShortRunJob] attribute to the class. 
    /// </summary>
    public static ManualConfig ShortRun => ManualConfig.CreateMinimumViable().AddJob(Job.ShortRun);
    /// <summary>
    /// Adds the [ShortRunJob] attribute to the class. Displays nothing to the console and 
    /// writes no log outputs (but does not interfere with exporters).
    /// </summary>
    public static ManualConfig SilentShortRun => ManualConfig.CreateEmpty().AddJob(Job.ShortRun).WithOptions(ConfigOptions.DisableLogFile);
    /// <summary>
    /// Adds the [MediumRunJob] attribute to the class. 
    /// </summary>
    public static ManualConfig MediumRun => ManualConfig.CreateMinimumViable().AddJob(Job.MediumRun);
    /// <summary>
    /// Adds the [MediumRunJob] attribute to the class. Displays nothing to the console and 
    /// writes no log outputs (but does not interfere with exporters).
    /// </summary>
    public static ManualConfig SilentMediumRun => ManualConfig.CreateEmpty().AddJob(Job.MediumRun).WithOptions(ConfigOptions.DisableLogFile);
    /// <summary>
    /// Adds the [LongRunJob] attribute to the class. 
    /// </summary>
    public static ManualConfig LongRun => ManualConfig.CreateMinimumViable().AddJob(Job.LongRun);
    /// <summary>
    /// Adds the [LongRunJob] attribute to the class. Displays nothing to the console and 
    /// writes no log outputs (but does not interfere with exporters).
    /// </summary>
    public static ManualConfig SilentLongRun => ManualConfig.CreateEmpty().AddJob(Job.LongRun).WithOptions(ConfigOptions.DisableLogFile);
}
