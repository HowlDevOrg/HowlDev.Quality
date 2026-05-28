using BenchmarkDotNet.Attributes;
using HowlDev.Quality.Benchmarking;

namespace BenchmarkingConsumer;

public class SampleBenchmarkInCode {
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

[DisassemblyDiagnoser]
[MemoryDiagnoser]
[ShortRunJob]
public class SampleBenchmarkWithAttr {
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

[DisassemblyDiagnoser]
[MemoryDiagnoser]
[ShortRunJob]
public class BenchWith1Params {
    [Params(3, 5)]
    public int N;
    [Params(1, 2)]
    public int N2;

    [Benchmark]
    public int AdditionWith5() {
        if (N < 4) {
            return 5 + N + N2;
        } else {
            BenchmarkFillers.FillMemory(3);
            return 5 + N + N2;
        }
    }
}
