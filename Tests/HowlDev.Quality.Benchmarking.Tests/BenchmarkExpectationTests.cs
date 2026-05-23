namespace HowlDev.Quality.Benchmarking.Tests;

public class BenchmarkExpectationTests {
    [Test]
    public async Task NanosecondTest() {
        var item = BenchmarkExpectations.ExpectedNanoseconds(10);
        await Assert.That(item.Nanoseconds).IsEqualTo(10);
        await Assert.That(item.Bytes).IsNull();
    }

    [Test]
    public async Task MicrosecondTest() {
        var item = BenchmarkExpectations.ExpectedMicroseconds(10);
        await Assert.That(item.Nanoseconds).IsEqualTo(10_000);
        await Assert.That(item.Microseconds).IsEqualTo(10);
        await Assert.That(item.Bytes).IsNull();
    }

    [Test]
    public async Task MillisecondTest() {
        var item = BenchmarkExpectations.ExpectedMilliseconds(10);
        await Assert.That(item.Nanoseconds).IsEqualTo(10_000_000);
        await Assert.That(item.Microseconds).IsEqualTo(10_000);
        await Assert.That(item.Milliseconds).IsEqualTo(10);
        await Assert.That(item.Bytes).IsNull();
    }

    [Test]
    public async Task BytesTest() {
        var item = BenchmarkExpectations.ExpectedBytes(10);
        await Assert.That(item.Bytes).IsEqualTo(10);
        await Assert.That(item.Nanoseconds).IsNull();
    }
}
