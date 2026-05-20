# HowlDev.Quality.Benchmarking

This suite builds on top of the [BenchmarkDotNet framework](https://www.nuget.org/packages/BenchmarkDotNet). 

## Setup

Given a standard class that you would build in their framework (for example): 

```cs
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
        BenchmarkFillers.FillMemory(8000);
        return value;
    }
}
```

The following is a snippet using this library that checks each function with a different set of information. 

```cs
BenchmarkValidator.For<SampleBenchmark>()
    .Expect("AdditionWithTimer", BenchmarkExpectations.ExpectedNanosecondsLessThan(60).WithBytes(0))
    .Expect("AdditionWithMemory1", BenchmarkExpectations.ExpectedBytes(64))
    .Expect("AdditionWithMemory2", BenchmarkExpectations.ExpectedBytes(7008).WithMicroseconds(0.3).WithMarginOfError(1.5))
    .Run();
```

This takes in a type like the original runner, then you provide `Expect()` statements with the method name and a `BenchmarkExpectation` object that will evaluate the result. You can combine time validation and GC collection validation into one validation check. 

To start, my system will reflect over the type and get all benchmarked methods and compare them to the list you provide. By default, it will pause on this screen if there are any conflicts (methods provided that don't exist or benchmarked methods that aren't validated) where you can check for errors. (To remove this behavior for a pipeline, pass `False` into the `.Run()` method). 

After it's checked, it will run the benchmark and save the results. It will safely check into the internal dictionary (so you can safely bypass any warnings by the validation step) and evaluate them. If they throw errors, they will store them in a local list so you can evaluate all failed benchmarks at once. 

If any were thrown, it will print those to the console and throw an AggregateException error and tell you to look at the console for detailed results, so you can evaluate if the code changes need to be reverted or your evaluations need to be updated to the new values. 

## Functions
### BenchmarkValidator
The base class of the library. Provides a static `.For<type>()` function to create a typed base class to then run Expectations on. 

### BenchmarkExpectations
This is how you define what you want the benchmark to do. Currently, it supports Time-based evaluations (either with a margin of error up or down or as strictly LessThan a given value) and Byte-based evaluations of the GC (these need to be exact numbers). In either case, the exception will show you what was checked and what a valid value would be (if it threw a Byte error that it wasn't an exact number, it will tell you the number to set it to). 

### BenchmarkFillers
This is a helper function to help test that might be useful to you, so I made it public. There are two methods, to either `FillTime()` or `FillBytes()`, that is a **very** rough approximation of something to fill time or bytes. 

Both functions take in the value (time or bytes to fill) and a tuning parameter to tune for your specific needs. I've set them to how they work on my laptop, but they may be different for you, so you can override them. 

The `FillTime()` parameter just runs a For loop for a given number of iterations, which I found more reliable than a Task.Delay or Thread.Sleep call. 

And you'd think that if you just returned a byte array of a given size, the GC would take up that much space. But it doesn't! Here's a small table of how some of them work on my machine. 
- Tuning parameter = 1
    - 3 -> 64
    - 500 -> 1056
    - 8000 -> 16048
- Tuning parameter = 2.3
    - 3 -> 64
    - 500 -> 496
    - 8000 -> 7008

So lower values evaluate higher (around 500, which is where I tuned it), and higher values evaluate lower. This is kinda bewildering to me, but this is what I could do. I suppose you could even tune the parameter on a per-method-call basis if you really needed it exact. Or maybe you can find a better solution.

### BenchmarkException
This is a custom exception for failures in the benchmark. First, it will throw errors if things are mistakenly configured (basically, if you call for a measurement of GC but don't include a `[MemoryDiagnoser]` attribute). 

Otherwise, it will throw an error with the following syntax for time errors: 

```
Method AdditionWithMemory3: Benchmark time out of bounds: actual=630.16, max=450, min=200.
```

And the following syntax for byte errors. 

```
Method AdditionWithMemory3: Benchmark memory was not equal to 7010 (exp 7008). Your changes resulted in higher memory use.
Method AdditionWithMemory3: Benchmark memory was not equal to 7000 (exp 7008). Update your function to (7000).
```

It will either tell you that your changes resulted in higher use (and thus your changes were bad) or that they were lower, which it tells you which value to set it to next. 

If both of these values fail in an evaluation, it will take both exceptions and call a static `Combine` function which joins the two error messages together, de-duplicating the method name and including a ` && ` separator between the two. It looks like this: 

```
Method AdditionWithTimer: Benchmark time out of bounds: actual=52.14, max=10, min=0. && Benchmark memory was not equal to 0 (exp 1). Update your function to (0).
```