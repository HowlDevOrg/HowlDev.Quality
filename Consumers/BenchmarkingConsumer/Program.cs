
using BenchmarkingConsumer;
using HowlDev.Quality.Benchmarking;

// Solo
// CustomBenchmarks.SampleBench.Run();
// CustomBenchmarks.Sample2Bench.Run();

// Optional as enumerable (if being built in code)
// List<IGroupBenchmark> benchmarks = [
//     CustomBenchmarks.SampleBench,
//     CustomBenchmarks.Sample2Bench
// ];

// Group
BenchmarkGroups.RunAll(GroupRunStrategy.RunAll, 
    CustomBenchmarks.SampleBench,
    CustomBenchmarks.Sample2Bench
);
