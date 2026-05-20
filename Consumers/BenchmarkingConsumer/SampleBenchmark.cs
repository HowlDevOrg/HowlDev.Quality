using BenchmarkDotNet.Attributes;
using HowlDev.Quality.Benchmarking;

namespace BenchmarkingConsumer;

[MemoryDiagnoser]
[ShortRunJob]
public class SampleBenchmark {
    [Benchmark]
    public int AdditionWithTimer() {
        int value = 5 + 6;
        BenchmarkFillers.FillTime(50);
        return value;
    }

    [Benchmark]
    public int AdditionWithMemory1() {
        int value = 5 + 6;
        BenchmarkFillers.FillMemory(3);
        return value;
    }

    [Benchmark]
    public int AdditionWithMemory2() {
        int value = 5 + 6;
        BenchmarkFillers.FillMemory(500);
        return value;
    }

    [Benchmark]
    public int AdditionWithMemory3() {
        int value = 5 + 6;
        BenchmarkFillers.FillMemory(8000);
        return value;
    }
}
