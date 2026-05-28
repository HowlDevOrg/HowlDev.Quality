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
    public BenchmarkException(string descript, int exp, double actual, bool memory) 
        : base(memory ? FormatMemoryMessage(descript, exp, actual) : FormatCodeSizeMessage(descript, exp, actual)) { }

    /// <summary>
    /// Send a Time message here. 
    /// </summary>
    public BenchmarkException(string descript, double actual, double max, double min)
        : base(FormatTimeMessage(descript, actual, max, min)) { }

    private static string FormatTimeMessage(string descript, double actual, double max, double min) =>
        $"{descript}: Benchmark time out of bounds: actual={Math.Round(actual, 2)}, max={Math.Round(max, 2)}, min={Math.Round(min, 2)}.";
    private static string FormatMemoryMessage(string descript, int exp, double actual) =>
        $"{descript}: Benchmark memory was not equal to: {actual} (exp: {exp}). {(actual < exp ? $"Update your function to ({actual})." : "Your changes resulted in higher memory use.")}";
    private static string FormatCodeSizeMessage(string descript, int exp, double actual) =>
        $"{descript}: Code size was not equal to: {actual} (exp: {exp}). {(actual < exp ? $"Update your function to ({actual})." : "Your changes resulted in higher code size.")}";

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

    /// <summary>
    /// Combines three exceptions together using nested calls to the original Combine method. 
    /// </summary>
    public static BenchmarkException Combine(BenchmarkException? ex1, BenchmarkException? ex2, BenchmarkException? ex3) {
        // Works as long as at least one is not null, which I should be checking. 
        if (ex1 is null) return Combine(ex2, ex3);
        if (ex2 is null) return Combine(ex1, ex3);
        if (ex3 is null) return Combine(ex1, ex2);
        return Combine(Combine(ex1, ex2), ex3);
    }

    /// <summary>
    /// Prepends a string to the start of the message.
    /// </summary>
    public static BenchmarkException Prepend(BenchmarkException ex1, string prefix) {
        return new BenchmarkException(prefix + ex1.Message);
    }
}
