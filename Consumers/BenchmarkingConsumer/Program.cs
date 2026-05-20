using BenchmarkDotNet.Running;
using BenchmarkingConsumer;
using HowlDev.Quality.Benchmarking;

Dictionary<string, BenchmarkExpectations> exp = new() {
    {"AdditionWithTimer", BenchmarkExpectations.ExpectedNanosecondsLessThan(60).WithBytes(0)},
    {"AdditionWithMemory1", BenchmarkExpectations.ExpectedBytes(64).WithNanoseconds(6).WithMarginOfError(1.5)},
    {"AdditionWithMemory2", BenchmarkExpectations.ExpectedBytes(496)},
    {"AdditionWithMemory3", BenchmarkExpectations.ExpectedBytes(7008).WithMicroseconds(0.25)}
};


BenchmarkRunner.Run<SampleBenchmark>()
    .Expect(exp);
