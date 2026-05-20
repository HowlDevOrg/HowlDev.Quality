using BenchmarkingConsumer;
using HowlDev.Quality.Benchmarking;

BenchmarkValidator.For<SampleBenchmark>()
    .Expect("AdditionWithTimer", BenchmarkExpectations.ExpectedNanosecondsLessThan(60).WithBytes(0))
    .Expect("AdditionWithMemory1", BenchmarkExpectations.ExpectedBytes(64))
    .Expect("AdditionWithMemory2", BenchmarkExpectations.ExpectedBytes(496))
    .Expect("AdditionWithMemory3", BenchmarkExpectations.ExpectedBytes(7008).WithMicroseconds(0.3).WithMarginOfError(1.5))
    .Run();
