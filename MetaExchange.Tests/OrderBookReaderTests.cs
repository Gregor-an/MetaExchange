using FluentAssertions;
using MetaExchange.Core.Services;

namespace MetaExchange.Tests
{
    public sealed class OrderBookReaderTests : IDisposable
    {
        private readonly IOrderBookReader _reader = new OrderBookReader();
        private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), $"metaexchange_tests_{Guid.NewGuid():N}");

        private const string ValidOrderBookJson = """{"AcqTime":"2024-01-01T00:00:00","Bids":[],"Asks":[]}""";

        public OrderBookReaderTests()
        {
            Directory.CreateDirectory(_tempDirectory);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, recursive: true);
            }
        }

        private string CreateTempFile(params string[] lines)
        {
            string path = Path.Combine(_tempDirectory, $"{Guid.NewGuid():N}.tmp");
            File.WriteAllLines(path, lines);

            return path;
        }

        // ---------------------------------------------------------------------------
        // ReadFromFile
        // ---------------------------------------------------------------------------

        [Fact]
        public void ReadFromFile_ReadsUpToRequestedCount()
        {
            var path = CreateTempFile(
                ValidOrderBookJson,
                ValidOrderBookJson,
                ValidOrderBookJson,
                ValidOrderBookJson,
                ValidOrderBookJson);

            var result = _reader.ReadFromFile(path, count: 3);

            result.Should().HaveCount(3);
        }

        [Fact]
        public void ReadFromFile_WhenCountExceedsAvailableLines_ReadsAll()
        {
            var path = CreateTempFile(
                ValidOrderBookJson,
                ValidOrderBookJson);

            var result = _reader.ReadFromFile(path, count: 10);

            result.Should().HaveCount(2);
        }

        [Fact]
        public void ReadFromFile_WhenFileIsEmpty_ReturnsEmptyList()
        {
            var path = CreateTempFile();

            var result = _reader.ReadFromFile(path, count: 5);

            result.Should().BeEmpty();
        }

        [Fact]
        public void ReadFromFile_WhenCountIsZero_ReturnsEmptyList()
        {
            var path = CreateTempFile(ValidOrderBookJson);

            var result = _reader.ReadFromFile(path, count: 0);

            result.Should().BeEmpty();
        }

        // ---------------------------------------------------------------------------
        // ReadFromFileAsync
        // ---------------------------------------------------------------------------

        [Fact]
        public async Task ReadFromFileAsync_ReadsUpToRequestedCount()
        {
            var path = CreateTempFile(
                ValidOrderBookJson,
                ValidOrderBookJson,
                ValidOrderBookJson,
                ValidOrderBookJson,
                ValidOrderBookJson);

            var result = await _reader.ReadFromFileAsync(path, count: 3);

            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task ReadFromFileAsync_WhenFileIsEmpty_ReturnsEmptyList()
        {
            var path = CreateTempFile();

            var result = await _reader.ReadFromFileAsync(path, count: 5);

            result.Should().BeEmpty();
        }
    }
}
