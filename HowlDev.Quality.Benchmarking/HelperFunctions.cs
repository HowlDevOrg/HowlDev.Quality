using System.Reflection;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Parameters;
using BenchmarkDotNet.Reports;

namespace HowlDev.Quality.Benchmarking;

internal static class HelperFunctions {
    public static void ValidateMethods<T>(bool pauseOnInvalid, List<string> providedMethods) {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Validating methods provided by you and by the type {typeof(T).Name}...");
        List<string> benchmarkedMethods = GetBenchmarkMethods<T>();

        bool invalid = false;

        IEnumerable<string> uncheckedBenchmarks = benchmarkedMethods.Where(a => !providedMethods.Contains(a));
        IEnumerable<string> unusedProvided = providedMethods.Where(a => !benchmarkedMethods.Contains(a));

        ConsoleColor problemColor = ConsoleColor.Yellow;

        if (uncheckedBenchmarks.Any()) {
            Console.WriteLine("Benchmarks without a validator: ");
            Console.ForegroundColor = problemColor;
            foreach (string item in uncheckedBenchmarks) {
                Console.WriteLine("- " + item);
            }

            invalid = true;
        }

        if (unusedProvided.Any()) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Provided methods that don't tie into a benchmarked method: ");
            Console.ForegroundColor = problemColor;
            foreach (string item in unusedProvided) {
                Console.WriteLine("- " + item);
            }

            invalid = true;
        }

        if (pauseOnInvalid && invalid) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Paused here. Press any key to continue. Press Ctrl + C to stop.");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("To not pause on validation problems, pass False to the Run method.");
            Console.ReadKey(true);
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Validation complete!");
        WriteBreaker();
    }

    public static void ValidateParams<T, P>(bool pauseOnInvalid, Dictionary<string, List<(P, BenchmarkExpectations)>> actions, string field) {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Validating params provided by you and by the type {typeof(T).Name}...");

        IEnumerable<FieldInfo> infos = typeof(T)
            .GetFields()
            .Where(a => a.GetCustomAttributes(typeof(ParamsAttribute), true) != null);

        if (infos.Count() > 1) throw new InvalidOperationException("Only one param field is currently supported.");
        if (!infos.Any()) throw new InvalidOperationException($"Class {typeof(T).Name} does not have any params.");

        bool invalid = false;
        foreach (FieldInfo item in infos) {
            var paramsAttr = item.GetCustomAttribute<ParamsAttribute>();
            if (paramsAttr != null) {
                if (item.Name != field) throw new InvalidDataException($"Provided param name {field} and seen name {item.Name} don't match.");
                List<P> parameters = [.. paramsAttr.Values.Select(a => (P)a!)];

                List<string> benchmarkedMethods = GetBenchmarkMethods<T>();
                List<string> checkMethods = [.. actions.Keys.Where(benchmarkedMethods.Contains)];

                foreach (string method in checkMethods) {
                    Console.ForegroundColor = ConsoleColor.White;
                    List<P> methodDefs = [.. actions[method].Select(a => a.Item1)];

                    IEnumerable<P> uncheckedParams = parameters.Where(a => !methodDefs.Contains(a));
                    IEnumerable<P> unusedProvided = methodDefs.Where(a => !parameters.Contains(a));

                    ConsoleColor problemColor = ConsoleColor.Yellow;

                    if (unusedProvided.Any() || uncheckedParams.Any()) {
                        invalid = true;
                        Console.WriteLine($"Method {method} had some parameter errors.");

                        if (uncheckedParams.Any()) {
                            Console.WriteLine("Parameters without a validator: ");
                            Console.ForegroundColor = problemColor;
                            foreach (P? param in uncheckedParams) {
                                Console.WriteLine("- " + param);
                            }
                        }

                        if (unusedProvided.Any()) {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("Provided params that don't tie into a param definition: ");
                            Console.ForegroundColor = problemColor;
                            foreach (P? param in unusedProvided) {
                                Console.WriteLine("- " + param);
                            }
                        }
                    }
                }
            }
        }

        if (pauseOnInvalid && invalid) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Paused here. Press any key to continue. Press Ctrl + C to stop.");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("To not pause on validation problems, pass False to the Run method.");
            Console.ReadKey(true);
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Validation complete!");
        WriteBreaker();
    }

    public static void DisplayAndThrowErrors(Summary result, Dictionary<string, BenchmarkExpectations> actions, string className) {
        DisplayErrorsIfExists(GetExceptions(result, actions, className));
    }

    public static void DisplayAndThrowErrors<T>(Summary result, Dictionary<string, List<(T, BenchmarkExpectations)>> actions, string className) {
        DisplayErrorsIfExists(GetExceptions(result, actions, className));
    }

    public static void DisplayErrorsIfExists(List<BenchmarkException> exceptions) {
        if (exceptions.Count > 0) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Exceptions thrown: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (BenchmarkException exception in exceptions) {
                Console.WriteLine("- " + exception.Message);
            }

            Console.ForegroundColor = ConsoleColor.White;
            WriteBreaker();
            Console.ForegroundColor = ConsoleColor.Red;
            throw new AggregateException("Exceptions were thrown. Scroll up to see the results.", exceptions);
        }
    }

    public static List<BenchmarkException> GetExceptions(Summary result, Dictionary<string, BenchmarkExpectations> actions, string className) {
        List<BenchmarkException> exceptions = [];
        foreach (BenchmarkReport report in result.Reports) {
            string methodName = report.BenchmarkCase.Descriptor.WorkloadMethod.Name;
            try {
                if (actions.TryGetValue(methodName, out BenchmarkExpectations? exp)) {
                    exp.Report(report, className);
                }
            } catch (Exception ex) {
                if (ex is BenchmarkException ex1) {
                    exceptions.Add(ex1);
                } else if (ex is InvalidDataException) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Somehow got an InvalidDataException in method {methodName}.");
                } else {
                    throw;
                }
            }
        }

        return exceptions;
    }

    public static List<BenchmarkException> GetExceptions<T>(Summary result, Dictionary<string, List<(T, BenchmarkExpectations)>> actions, string className) {
        List<BenchmarkException> exceptions = [];
        foreach (BenchmarkReport report in result.Reports) {
            string methodName = report.BenchmarkCase.Descriptor.WorkloadMethod.Name;
            (T, BenchmarkExpectations) match = (default(T), BenchmarkExpectations.ExpectedBytes(0))!;
            try {
                if (actions.TryGetValue(methodName, out List<(T, BenchmarkExpectations)>? exp)) {
                    for (int j = 0; j < report.BenchmarkCase.Parameters.Count; j++) {
                        ParameterInstance item = report.BenchmarkCase.Parameters[j];
                        match = exp.FirstOrDefault(a => EqualityComparer<T>.Default.Equals(a.Item1, (T)item.Value));

                        match.Item2.Report(report, className);
                    }
                }
            } catch (Exception ex) {
                if (ex is BenchmarkException ex1) {
                    exceptions.Add(BenchmarkException.Prepend(ex1, $"(Param {match.Item1})"));
                } else if (ex is InvalidDataException) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Somehow got an InvalidDataException in method {methodName}.");
                } else {
                    throw;
                }
            }
        }

        return exceptions;
    }

    public static List<string> GetBenchmarkMethods<T>() {
        return [.. typeof(T)
            .GetMethods()
            .Where(a => a.GetCustomAttribute<BenchmarkAttribute>() != null)
            .Select(a => a.Name)];
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBreaker() {
        Console.ResetColor();
        Console.WriteLine("================");
    }
}
