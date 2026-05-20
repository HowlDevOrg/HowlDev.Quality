using BenchmarkDotNet.Reports;

namespace HowlDev.Quality.Benchmarking;

/// <summary/>
public static class BenchmarkExtensions {
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
}
