using System.Globalization;
using System.Text.Json;
using MetaExchange.Core.Models;
using MetaExchange.Core.Services;

namespace MetaExchange.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: MetaExchange.Console <buy|sell> <amount>");
                Console.WriteLine("Example: MetaExchange.Console buy 9");
                return;
            }

            if (!TryParseOrderSide(args[0], out OrderSide side))
            {
                Console.WriteLine("Error: first argument must be 'buy' or 'sell'.");
                return;
            }

            if (!decimal.TryParse(args[1], CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Error: amount must be a positive number.");
                return;
            }

            AppSettings? settings = LoadSettings();
            if (settings is null) return;

            string dataFile = Path.IsPathRooted(settings.DataFile)
                ? settings.DataFile
                : Path.Combine(AppContext.BaseDirectory, settings.DataFile);

            if (!File.Exists(dataFile))
            {
                Console.WriteLine($"Error: data file not found: {dataFile}");
                return;
            }

            List<OrderBook> orderBooks = OrderBookReader.ReadFromFile(dataFile, settings.Exchanges.Count);
            if (orderBooks.Count == 0)
            {
                Console.WriteLine("Error: no order books loaded from file.");
                return;
            }

            List<Exchange> exchanges = settings.Exchanges
                .Take(orderBooks.Count)
                .Select((config, i) => new Exchange
                {
                    Id = config.Id,
                    EurBalance = config.EurBalance,
                    BtcBalance = config.BtcBalance,
                    OrderBook = orderBooks[i],
                })
                .ToList();

            IExecutionPlanService service = new ExecutionPlanService();
            ExecutionPlan plan = service.BuildExecutionPlan(exchanges, side, amount);

            PrintPlan(plan, side, amount);
        }

        private static bool TryParseOrderSide(string value, out OrderSide side)
        {
            return Enum.TryParse(value.Trim(), ignoreCase: true, out side);
        }

        private static AppSettings? LoadSettings()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(path))
            {
                Console.WriteLine($"Error: appsettings.json not found at {path}");
                return null;
            }
            try
            {
                return JsonSerializer.Deserialize<AppSettings>(
                    File.ReadAllText(path),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error: failed to parse appsettings.json. {ex.Message}");
                return null;
            }
        }

        private static void PrintPlan(ExecutionPlan plan, OrderSide side, decimal requested)
        {
            Console.WriteLine($"Requested order: {side.ToString().ToUpperInvariant()} {requested} BTC");
            Console.WriteLine();

            if (plan.Orders.Count == 0)
            {
                Console.WriteLine("No executable orders found.");
            }
            else
            {
                Console.WriteLine("Orders to execute:");

                foreach (var order in plan.Orders)
                {
                    Console.WriteLine(
                        $"- {order.Side} {order.Amount:F8} BTC on {order.ExchangeId} " +
                        $"at {order.Price:F2} EUR/BTC. Total: {order.Total:F2} EUR");
                }
            }

            Console.WriteLine($"Executed BTC: {plan.TotalBtc:F8}");
            Console.WriteLine($"Total EUR: {plan.TotalEur:F2}");
            Console.WriteLine($"Average price: {plan.AveragePrice:F2} EUR/BTC");
            Console.WriteLine($"Status: {plan.Status}");
        }
    }

    internal record ExchangeConfig(string Id, decimal EurBalance, decimal BtcBalance);
    internal record AppSettings(string DataFile, List<ExchangeConfig> Exchanges);
}
