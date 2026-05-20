namespace HowlDev.Quality.Benchmarking;

/// <summary>
/// A collection of items to artificially inflate benchmarks 
/// for testing purposes. 
/// </summary>
public static class BenchmarkFillers {
    /// <summary>
    /// A rough approximation of something that extends time without performing
    /// other actions. 
    /// </summary>
    /// <param name="nanoseconds">Nanoseconds to wait</param>
    /// <param name="tuningParameter">Tuning parameter</param>
    public static void FillTime(int nanoseconds, double tuningParameter = 4) {
        int actualLoops = (int)(nanoseconds * tuningParameter);
        for (int i = 0; i < actualLoops; i++) { }
    }

    /// <summary>
    /// A rough approximation of something that extends memory usage. <br/>
    /// It seems to be accurate around 400, with things lower reading higher
    /// and things higher reading lower. You may need to play around with it. 
    /// </summary>
    /// <param name="bytes">Bytes to fill</param>
    /// <param name="tuningParameter">Tuning parameter</param>
    public static byte[] FillMemory(int bytes, double tuningParameter = 2.3) {
        return [.. new byte[(int)(bytes / tuningParameter)]]; // This division operator is a bit of voodoo, I'm not sure why it works like that.
    }
}
