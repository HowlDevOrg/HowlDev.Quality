
using BenchmarkingConsumer;
using HowlDev.Quality.Benchmarking;

// Solo
// CustomBenchmarks.SampleBenchmarkInCode.Run();
// CustomBenchmarks.SampleBenchmarkWithAttr.Run();

// Group
BenchmarkGroups.RunAll(GroupRunStrategy.RunAll, 
    CustomBenchmarks.SampleBenchmarkInCode,
    CustomBenchmarks.SampleBenchmarkWithAttr
);

// Optional as enumerable (if being built in code)
// List<IGroupBenchmark> benchmarks = [
//     CustomBenchmarks.SampleBenchmarkInCode,
//     CustomBenchmarks.SampleBenchmarkWithAttr
// ];
// BenchmarkGroups.RunAll(GroupRunStrategy.RunAll, benchmarks);
