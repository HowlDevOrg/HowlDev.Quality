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
    public static void FillTime(int nanoseconds) {
        int actualLoops = nanoseconds * 4;
        for (int i = 0; i < actualLoops; i++) { }
    }

    /// <summary>
    /// A rough approximation of something that extends memory usage. <br/>
    /// It seems to be accurate around 400, with things lower being higher
    /// and things higher being lower. You may need to play around with it. 
    /// </summary>
    public static byte[] FillMemory(int bytes) {
        return [..new byte[(int)(bytes / 2.3)]]; // This division operator is a bit of voodoo, I'm not sure why it works like that.
    }
}
