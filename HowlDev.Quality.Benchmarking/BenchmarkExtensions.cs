using BenchmarkDotNet.Reports;

namespace HowlDev.Quality.Benchmarking;

/// <summary/>
public static class BenchmarkExtensions {
    /// <summary>
    /// For each function 
    /// Throws an exception if the value is out of that range. 
    /// </summary>
    /// <param name="summary">Benchmark result.</param>
    /// <param name="dict">Dictionary of expectations</param>
    /// <exception cref="BenchmarkException"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public static Summary Expect(this Summary summary, Dictionary<string, BenchmarkExpectations> dict) {
        foreach (BenchmarkReport item in summary.Reports) {
            string methodName = item.BenchmarkCase.Descriptor.WorkloadMethod.Name;
            Validate(item, dict[methodName], methodName);
        }

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Completed Expect function!");

        return summary;
    }

    /// <summary>
    /// Help debug the inputs for a given benchmark suite. <br/>
    /// 
    /// Provides the method name (key of your dictionary), the mean, and 
    /// the allocated memory. 
    /// </summary>
    /// <param name="summary"></param>
    public static void DebugDisplay(this Summary summary) {
        Console.WriteLine($"======{summary.Title.Split('-')[0]}======");
        for (int i = 0; i < summary.Reports.Length; i++) {
            Console.WriteLine("Method name: " + summary.Reports[i].BenchmarkCase.Descriptor.WorkloadMethod.Name);
            Console.WriteLine("Mean (in ns): " + summary.Reports[i].ResultStatistics!.Mean);
            try {
                Console.WriteLine("GC Allocation: " + summary.Reports[i].Metrics["Allocated Memory"].Value);
            } catch {
                Console.WriteLine("GC Allocation not recorded. Include [MemoryDiagnoser] attribute to validate.");
            }
            Console.WriteLine("------------------------");
        }

        Console.WriteLine("============");
    }

    private static void Validate(BenchmarkReport report, BenchmarkExpectations exp, string methodName) {
        try {
            exp.IsValidTime(report.ResultStatistics!.Mean);

            if (exp.Bytes is not null) {
                double actBytes;
                try {
                    actBytes = report.Metrics["Allocated Memory"].Value;
                } catch {
                    throw new BenchmarkException($"Did not find a memory allocation for title: {report.BenchmarkCase}");
                }

                exp.IsValidBytes(actBytes);
            }
        } catch {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("==================");
            Console.WriteLine($"Couldn't complete validation for method: {methodName}");
            Console.ForegroundColor = ConsoleColor.White;
            throw;
        }
    }
}

/* 
GOALS: 
- Explosion of methods!
    - Need methods that take in both a single value for single calls and Dict for multiple calls
        - Bunch of options here. But as above, I should split into different method calls for 
            the time check and for the GC check. 
    - I should fail fast; so I shouldn't throw a benchmarking error if there's a method name in the 
        benchmark suite that doesn't have an appropriate key (it should validate that first). 
*/

