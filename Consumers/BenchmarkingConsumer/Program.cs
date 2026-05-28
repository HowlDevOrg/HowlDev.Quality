
using BenchmarkingConsumer;
using HowlDev.Quality.Benchmarking;

// Temporary notes
// IEnumerable<FieldInfo> infos = typeof(BenchWith1Params)
//     .GetFields()
//     .Where(a => a.GetCustomAttributes(typeof(ParamsAttribute), true) != null);

// List<string> fields = infos
//     .Select(a => a.Name)
//     .ToList();

// foreach (var item in fields) {
//     Console.WriteLine(item);
// }

// foreach (FieldInfo item in infos) {
//     var paramsAttr = item.GetCustomAttribute<ParamsAttribute>();
//     if (paramsAttr != null) {
//         Console.WriteLine($"Field: {item.Name}");
//         Console.WriteLine($"  Params: {string.Join(", ", paramsAttr.Values)}");
//     }
// }

// Summary sum = BenchmarkRunner.Run<BenchWith1Params>();
// for (int i = 0; i < sum.Reports.Length; i++) {
//     Console.WriteLine(i);
//     BenchmarkReport rep = sum.Reports[i];
//     for (int j = 0; j < rep.BenchmarkCase.Parameters.Count; j++) {
//         var item = rep.BenchmarkCase.Parameters[j];
//         Console.WriteLine(item.Name);
//         Console.WriteLine(item.Value);
//     }

//     Console.WriteLine("=========");
// }

// Solo
// CustomBenchmarks.SampleBenchmarkInCode.Run();
// CustomBenchmarks.SampleBenchmarkWithAttr.Run();

// Group
BenchmarkGroups.RunAll(GroupRunStrategy.RunAll, 
    CustomBenchmarks.SampleBenchmarkInCode,
    CustomBenchmarks.SampleBenchmarkWithAttr
);

// Optional as enumerable (if being built in code)
// List<IGroupBenchmark> benchmarks = [
//     CustomBenchmarks.SampleBenchmarkInCode,
//     CustomBenchmarks.SampleBenchmarkWithAttr
// ];
// BenchmarkGroups.RunAll(GroupRunStrategy.RunAll, benchmarks);
