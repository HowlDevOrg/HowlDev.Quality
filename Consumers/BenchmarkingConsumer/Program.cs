using BenchmarkingConsumer;
using HowlDev.Quality.Benchmarking;

BenchmarkValidator.For<SampleBenchmark>()
    .Expect("AdditionWithTimer", BenchmarkExpectations.ExpectedNanosecondsLessThan(60).WithBytes(0))
    .Expect("AdditionWithMemory", BenchmarkExpectations.ExpectedBytes(64))
    .Run();
