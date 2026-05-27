using BenchmarkDotNet.Attributes;
using HowlDev.Quality.Benchmarking;

namespace BenchmarkingConsumer;

public class SampleBenchmark {
    [Benchmark]
    public int AdditionWithTimer() {
        int value = 5 + 6;
        BenchmarkFillers.FillTime(50);
        return value;
    }

    [Benchmark]
    public int AdditionWithMemory() {
        int value = 5 + 6;
        BenchmarkFillers.FillMemory(3);
        return value;
    }
}

[MemoryDiagnoser]
[ShortRunJob]
public class SampleBenchmark2 {
    [Benchmark]
    public int AdditionWithTimer() {
        int value = 5 + 6;
        BenchmarkFillers.FillTime(10);
        return value;
    }

    [Benchmark]
    public int AdditionWithMemory() {
        int value = 5 + 6;
        BenchmarkFillers.FillMemory(100);
        return value;
    }
}
