namespace HowlDev.Quality.Benchmarking.Tests;

public class Tests {
    [Test]
    public async Task Test() {
        int value = 5 + 6;
        await Assert.That(value).IsEqualTo(11);
    }
}
