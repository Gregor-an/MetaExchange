using System.Text.Json;
using MetaExchange.Core.Models;

namespace MetaExchange.Core.Services
{
    /// <summary>
    /// Reads order books from a line-delimited JSON file.
    /// </summary>
    public static class OrderBookReader
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Reads up to <paramref name="count"/> order books from the specified file.
        /// Each line in the file is expected to be a JSON-serialized <see cref="OrderBook"/>.
        /// </summary>
        /// <param name="filePath">Path to the order books data file.</param>
        /// <param name="count">Maximum number of order books to read.</param>
        /// <returns>A list of deserialized <see cref="OrderBook"/> instances.</returns>
        public static List<OrderBook> ReadFromFile(string filePath, int count)
        {
            var result = new List<OrderBook>();

            foreach (var line in File.ReadLines(filePath).Take(count))
            {
                var orderBook = JsonSerializer.Deserialize<OrderBook>(line, JsonOptions);
                if (orderBook != null)
                    result.Add(orderBook);
            }

            return result;
        }

        /// <summary>
        /// Asynchronously reads up to <paramref name="count"/> order books from the specified file.
        /// Each line in the file is expected to be a JSON-serialized <see cref="OrderBook"/>.
        /// </summary>
        /// <param name="filePath">Path to the order books data file.</param>
        /// <param name="count">Maximum number of order books to read.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        /// <returns>A list of deserialized <see cref="OrderBook"/> instances.</returns>
        public static async Task<List<OrderBook>> ReadFromFileAsync(string filePath, int count, CancellationToken cancellationToken = default)
        {
            var result = new List<OrderBook>();

            await foreach (var line in File.ReadLinesAsync(filePath).WithCancellation(cancellationToken))
            {
                if (result.Count >= count) break;

                var orderBook = JsonSerializer.Deserialize<OrderBook>(line, JsonOptions);
                if (orderBook != null)
                    result.Add(orderBook);
            }

            return result;
        }
    }
}
