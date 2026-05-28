using HowlDev.Quality.Benchmarking;
using HowlDev.Quality.Benchmarking.Validators;

namespace BenchmarkingConsumer;

public static class CustomBenchmarks {
    public static IBenchmarkValidator SampleBenchmarkInCode => BenchmarkValidator.For<SampleBenchmarkInCode>()
        .WithProfile(BenchmarkProfiles.ShortRun)
        .Expect("AdditionWithTimer", BenchmarkExpectations.ExpectedNanosecondsLessThan(60).WithBytes(0).WithCodeSize(20))
        .Expect("AdditionWithMemory", BenchmarkExpectations.ExpectedBytes(64).WithCodeSize(1462))
        .WithMemoryDiagnoser()
        .WithDisassemblyOutput()
        .WithGithubExporter();

    public static IBenchmarkValidator SampleBenchmarkWithAttr => BenchmarkValidator.For<SampleBenchmarkWithAttr>()
        .Expect("AdditionWithTimer", BenchmarkExpectations.ExpectedNanosecondsLessThan(30).WithBytes(0).WithCodeSize(20))
        .Expect("AdditionWithMemory", BenchmarkExpectations.ExpectedBytes(144).WithCodeSize(1436));
}
