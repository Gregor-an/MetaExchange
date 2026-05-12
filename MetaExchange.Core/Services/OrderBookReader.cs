using System.Text.Json;
using MetaExchange.Core.Models;

namespace MetaExchange.Core.Services
{
    public static class OrderBookReader
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

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
