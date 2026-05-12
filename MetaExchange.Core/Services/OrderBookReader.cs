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
    }
}
