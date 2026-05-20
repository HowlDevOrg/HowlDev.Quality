namespace HowlDev.Quality.Benchmarking;

/// <summary>
/// Exceptions returned by HowlDev.Quality.Benchmarking. <br/>
/// 
/// A benchmark was out of bounds in either direction in speed 
/// or memory usage. 
/// </summary>
public class BenchmarkException : Exception {
    /// <summary>
    /// Send an arbitrary string here.
    /// </summary>
    /// <param name="value"></param>
    public BenchmarkException(string value) : base(value) { }
    /// <summary>
    /// Send a Memory message here. 
    /// </summary>
    public BenchmarkException(int exp, double actual) : base(FormatMemoryMessage(exp, actual)) { }

    /// <summary>
    /// Send a Time message here. 
    /// </summary>
    public BenchmarkException(double actual, double max, double min)
        : base(FormatTimeMessage(actual, max, min)) { }

    private static string FormatTimeMessage(double actual, double max, double min) =>
        $"Benchmark time out of bounds: actual={Math.Round(actual, 2)}, max={Math.Round(max, 2)}, min={Math.Round(min, 2)}";
    private static string FormatMemoryMessage(int exp, double actual) =>
        $"Benchmark memory was not equal to: {actual} (exp: {exp}). {(actual < exp ? $"Update your function to ({actual})." : "Your changes resulted in higher memory use.")}";
}
