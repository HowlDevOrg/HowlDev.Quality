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

    public static IBenchmarkValidator OneParamBenchInCode => BenchmarkValidator.For<BenchWith1ParamsInCode>()
        .ForParams<int>("N")
        .WithProfile(BenchmarkProfiles.ShortRun)
        .Expect("AdditionWith5", 2, BenchmarkExpectations.ExpectedNanosecondsLessThan(30).WithBytes(0).WithCodeSize(49))
        .Expect("AdditionWith5", 4, BenchmarkExpectations.ExpectedBytes(64).WithCodeSize(1498))
        .WithMemoryDiagnoser()
        .WithDisassemblyOutput()
        .WithGithubExporter();

    public static IBenchmarkValidator SampleBenchmarkWithAttr => BenchmarkValidator.For<SampleBenchmarkWithAttr>()
        .Expect("AdditionWithTimer", BenchmarkExpectations.ExpectedNanosecondsLessThan(30).WithBytes(0).WithCodeSize(20))
        .Expect("AdditionWithMemory", BenchmarkExpectations.ExpectedBytes(144).WithCodeSize(1436));

    public static IBenchmarkValidator OneParamBenchWithAttr => BenchmarkValidator.For<BenchWith1Params>()
        .ForParams<int>("N")
        .Expect("AdditionWith5", 3, BenchmarkExpectations.ExpectedNanosecondsLessThan(30).WithBytes(0).WithCodeSize(49))
        .Expect("AdditionWith5", 5, BenchmarkExpectations.ExpectedBytes(64).WithCodeSize(1498));
}
