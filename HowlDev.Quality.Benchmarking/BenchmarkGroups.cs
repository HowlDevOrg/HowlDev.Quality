namespace HowlDev.Quality.Benchmarking;

/// <summary>
/// Allows the running of multiple <c>BenchmarkValidator</c>s in 
/// sequence without throwing errors in-between. Useful for debugging
/// while still allowing for more features. 
/// </summary>
public static class BenchmarkGroups {
    /// <summary>
    /// Runs the benchmarks given the strategy enum provided.
    /// </summary>
    public static void RunAll(GroupRunStrategy strategy, params IGroupBenchmark[] benchmarks) {
        RunAll(strategy, benchmarks.ToList());
    }
    
    /// <summary>
    /// Runs the benchmarks given the strategy enum provided.
    /// </summary>
    public static void RunAll(GroupRunStrategy strategy, IEnumerable<IGroupBenchmark> benchmarks) {
        if (strategy == GroupRunStrategy.RunAll) {
            foreach (IGroupBenchmark bench in benchmarks) {
                bench.Validate();
            }
        }

        List<BenchmarkException> exceptions = [];
        foreach (IGroupBenchmark bench in benchmarks) {
            List<BenchmarkException> runner = bench.RunAndCollectExceptions();
            if (strategy == GroupRunStrategy.ThrowOnFirstError) {
                HelperFunctions.DisplayErrorsIfExists(runner);
            } else {
                exceptions.AddRange(runner);
            }
        }

        HelperFunctions.DisplayErrorsIfExists(exceptions);
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Completed group benchmark runs!");
    }
}


/// <summary>
/// Which strategy the BenchmarkGroup should run. 
/// </summary>
public enum GroupRunStrategy {
    /// <summary>
    /// Run all benchmarks sequentially, throw any errors and console stuff at the end of 
    /// all benchmarks. <br/>
    /// Iterates through the list and validates all of their methods (pausing on the console), 
    /// then runs through again and runs their benchmarks.
    /// </summary>
    RunAll,
    /// <summary>
    /// Run sequentially, throw and stop the run on first error. Useful for CI pipelines
    /// or when you need to protect CPU runtime. <br/>
    /// Does not validate the methods and just runs the benchmarks.
    /// </summary>
    ThrowOnFirstError
}
