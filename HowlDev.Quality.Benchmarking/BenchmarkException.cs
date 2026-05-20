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
    public BenchmarkException(string method, int exp, double actual) : base(FormatMemoryMessage(method, exp, actual)) { }

    /// <summary>
    /// Send a Time message here. 
    /// </summary>
    public BenchmarkException(string method, double actual, double max, double min)
        : base(FormatTimeMessage(method, actual, max, min)) { }

    private static string FormatTimeMessage(string method, double actual, double max, double min) =>
        $"Method {method}: Benchmark time out of bounds: actual={Math.Round(actual, 2)}, max={Math.Round(max, 2)}, min={Math.Round(min, 2)}.";
    private static string FormatMemoryMessage(string method, int exp, double actual) =>
        $"Method {method}: Benchmark memory was not equal to: {actual} (exp: {exp}). {(actual < exp ? $"Update your function to ({actual})." : "Your changes resulted in higher memory use.")}";

    /// <summary>
    /// Combines the two error messages into one. If one or the other is null, simple returns 
    /// that exception. <br/>
    /// Throws an InvalidCastException if both are null. 
    /// </summary>
    /// <exception cref="InvalidCastException"></exception>
    public static BenchmarkException Combine(BenchmarkException? left, BenchmarkException? right) {
        if (left is not null && right is not null)
            return new(left.Message + " &&" + string.Join("", right.Message.Split(':')[1..]));
        else if (left is not null) return left;
        else if (right is not null) return right;
        throw new InvalidCastException("Can't combine two null exceptions.");
    }
}
