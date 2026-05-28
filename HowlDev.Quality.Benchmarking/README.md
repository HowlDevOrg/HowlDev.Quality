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

## Everything in one place

v0.2 provides the feature of including things you'd normally need to put in attributes into the code themselves, to make them easier to change. 

All of this applies to a BenchmarkValidator object that you've applied the `.WithProfile()` function to, which changes the type. Nothing changed for the default (though you can now pass in your own custom config, finally). If you do use `.WithProfile()`, it's recommended to remove any class-level attributes, as seen below. 

There are a now a few default runs to quick switch speeds (Short vs. Medium), enable/disable logging to the console, disable creating log files, enable Memory and Disassembly diagnostics, and add exporters (Github is named, but you can pass in any `IExporter` function). 

To recap, the new benchmark file looks like this: 

```cs
namespace BenchmarkingConsumer;

public class SampleBenchmark {
    [Benchmark] // These are still required
    public int AdditionWithTimer() {
        int value = 5 + 6;
        BenchmarkFillers.FillTime(50);
        return value;
    }

    // ...
}
```

And the new options look like so: 

```cs
BenchmarkValidator.For<SampleBenchmark>()
    .Expect("AdditionWithTimer", BenchmarkExpectations.ExpectedNanosecondsLessThan(60).WithBytes(0))
    .Expect("AdditionWithMemory", BenchmarkExpectations.ExpectedBytes(64))
    // .WithProfile creates a new object type with different functions, and overrides many of the configs.
    // It could cause unexpected results if used with class-level attributes on the benchmark, so make sure 
    // to remove those for consistent results. 
    .WithProfile(BenchmarkProfiles.SilentShortRun) // These 4 lines can go before or after the .Expect functions
    .WithMemoryDiagnoser()
    .WithDisassemblyOutput()
    .WithGithubExporter()
    // .WithoutLogOutput() // Not needed with Silent calls. 
    .Run();
```

I provided 3 job lengths by default, Short, Medium, and Long (the same defaults from the library). They also contain a default and a Silent version, which the silent version removes the console output of the Runner (not the exception or other validation logic) and removes the log file. I've made a small table to help. 

| | Want console logging | Don't want console logging |
| --- | --- | --- | 
| Want log files | ShortRun | N/A |
| Don't want log files | ShortRun with `.WithoutLogOutput()` | SilentShortRun | 

---

## Groups and Params

v0.3 provides Group and `[Params]` support. 

You know how annoying it is when something throws an error in a CI pipeline and then you go fix that one thing, then you run it again and something later in the pipeline breaks? 

That can happen pretty easily with sequential BenchmarkValidator runs. So, there's now a static `BenchmarkGroups` class that has a RunAll method and two different enums to choose from, whether you are trying to debug it and it should run all of them, then throw errors, or it should save CPU cycles and throw on the first error. 

Briefly, before showing you the group code, first you should know that everything branching off of `BenchmarkValidator.For<>()` now implements a shared interface, `IBenchmarkValidator`, which has one subset for runners (running individual ones with the `.Run()` function) and one interface for this Group features, which collects all exceptions and has to run a bit differently. 

Because they share one interface, it's much easier to write a static class where you put all the numbers and expectations, then call them in one-liners that can be easily commented out for debugging purposes. 

With that said, the groups run as a `params IBenchmarkValidator` or as `IEnumerable<IBenchmarkValidator>`, whichever is easier for you to make. The enum is the first parameter, then as many benchmarks as you want to run. 

```cs
// Group
BenchmarkGroups.RunAll(GroupRunStrategy.RunAll, 
    CustomBenchmarks.SampleBenchmarkInCode,
    CustomBenchmarks.SampleBenchmarkWithAttr
);
```

The above snippet will take those in, run the validations at the start (any method name mismatches), then runs them sequentially. If there are any errors in any runs, they throw at the end with the class name (new!) and method name attached to be easy to find. 

### Params

I now support.. 1 whole `[Params]` parameter! It requires the type and name of the parameter in the call, and this enforces one of those types to be in your Expect call (so you can write different expectations for the same method but a different parameter). 

You do this with the `.ForParams<>("")` call. Using the Static explanation from above, here's an example of what it looks like, with some comments. This version runs via attributes and no profile, which is shorter for demonstration here. 

```cs
public static IBenchmarkValidator OneParamBenchWithAttr => BenchmarkValidator.For<BenchWith1Params>()
    .ForParams<int>("N")
    // For the method AdditionWith5, and where the parameter is equal to 3
    .Expect("AdditionWith5", 3, BenchmarkExpectations.ExpectedNanosecondsLessThan(30).WithBytes(0).WithCodeSize(49))
    // For the method AdditionWith5, and where the parameter is equal to 5
    .Expect("AdditionWith5", 5, BenchmarkExpectations.ExpectedBytes(64).WithCodeSize(1498));
```

You may wonder why their results are so drastically different, and that's so I could make sure that one was different than the other. 

```cs
public class BenchWith1Params {
    [Params(3, 5)]
    public int N;

    [Benchmark]
    public int AdditionWith5() {
        if (N < 4) {
            return 5 + N;
        } else {
            BenchmarkFillers.FillMemory(3);
            return 5 + N;
        }
    }
}
```

### Other things

This added in a restriction to the way you build with new profiles, so now, the selection of what the benchmark does and will run needs to be decided directly after the `.For<>()` call. Once you call Expect, you're limited to only Expect and Run. Once you call WithProfile, you're limited to that configuration (and can put subsequent calls wherever). This should be pretty easy to reorder if you're getting errors. 

You also need to call `.ForParams<T>("N")` directly after, then you can choose the Expect or WithProfile option in the same way. It was just a minor explosion of types that I don't really know how to fix right now, so I stuck with one before writing/duplicating too much. 
