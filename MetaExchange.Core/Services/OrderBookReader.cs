using System.Text.Json;
using MetaExchange.Core.Models;

namespace MetaExchange.Core.Services
{
    /// <summary>
    /// Reads order books from a line-delimited JSON file.
    /// </summary>
    public sealed class OrderBookReader : IOrderBookReader
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <inheritdoc/>
        public List<OrderBook> ReadFromFile(string filePath, int count)
        {
            var result = new List<OrderBook>();

            foreach (var line in File.ReadLines(filePath).Take(count))
            {
                var orderBook = JsonSerializer.Deserialize<OrderBook>(line, JsonOptions);
                if (orderBook != null)
                {
                    result.Add(orderBook);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public async Task<List<OrderBook>> ReadFromFileAsync(string filePath, int count, CancellationToken cancellationToken = default)
        {
            var result = new List<OrderBook>();

            await foreach (var line in File.ReadLinesAsync(filePath, cancellationToken))
            {
                if (result.Count >= count) break;

                var orderBook = JsonSerializer.Deserialize<OrderBook>(line, JsonOptions);
                if (orderBook != null)
                {
                    result.Add(orderBook);
                }
            }

            return result;
        }
    }
}
