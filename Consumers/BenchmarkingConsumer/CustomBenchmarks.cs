using HowlDev.Quality.Benchmarking;

namespace BenchmarkingConsumer; 

public static class CustomBenchmarks {
    public static IBenchmarkValidator SampleBench => BenchmarkValidator.For<SampleBenchmark>()
        .Expect("AdditionWithTimer", BenchmarkExpectations.ExpectedNanosecondsLessThan(60).WithBytes(0))
        .Expect("AdditionWithMemory", BenchmarkExpectations.ExpectedBytes(64))
        .WithProfile(BenchmarkProfiles.ShortRun)
        .WithMemoryDiagnoser()
        .WithDisassemblyOutput()
        .WithGithubExporter();
}
