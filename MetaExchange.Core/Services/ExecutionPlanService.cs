using MetaExchange.Core.Models;

namespace MetaExchange.Core.Services
{
    /// <summary>
    /// Builds the best execution plan across multiple exchanges.
    /// </summary>
    public class ExecutionPlanService : IExecutionPlanService
    {
        /// <inheritdoc/>
        public ExecutionPlan BuildExecutionPlan(IReadOnlyCollection<Exchange> exchanges, OrderSide side, decimal btcAmount)
        {
            if (btcAmount <= 0)
                throw new ArgumentException("BTC amount must be greater than zero.", nameof(btcAmount));

            return side switch
            {
                OrderSide.Buy  => Buy(exchanges, btcAmount),
                OrderSide.Sell => Sell(exchanges, btcAmount),
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
            };
        }

        private static ExecutionPlan Buy(IReadOnlyCollection<Exchange> exchanges, decimal btcAmount)
        {
            decimal remaining = btcAmount;
            List<ExecutionOrder> orders = [];

            var allAsks = exchanges
                .SelectMany(e => e.OrderBook.Asks.Select(a => (Exchange: e, Ask: a.Order)))
                .Where(x => x.Ask.Price > 0 && x.Ask.Amount > 0)
                .OrderBy(x => x.Ask.Price);

            foreach (var (exchange, ask) in allAsks)
            {
                if (remaining <= 0) break;

                decimal amount = Math.Min(remaining, Math.Min(ask.Amount, exchange.EurBalance / ask.Price));
                if (amount <= 0) continue;

                orders.Add(new ExecutionOrder
                {
                    ExchangeId = exchange.Id,
                    Side = OrderSide.Buy,
                    Amount = amount,
                    Price = ask.Price
                });

                exchange.EurBalance -= amount * ask.Price;
                remaining -= amount;
            }

            return BuildPlan(orders, btcAmount, remaining);
        }

        private static ExecutionPlan Sell(IReadOnlyCollection<Exchange> exchanges, decimal btcAmount)
        {
            decimal remaining = btcAmount;
            List<ExecutionOrder> orders = [];

            var allBids = exchanges
                .SelectMany(e => e.OrderBook.Bids.Select(b => (Exchange: e, Bid: b.Order)))
                .Where(x => x.Bid.Price > 0 && x.Bid.Amount > 0)
                .OrderByDescending(x => x.Bid.Price);

            foreach (var (exchange, bid) in allBids)
            {
                if (remaining <= 0) break;

                decimal amount = Math.Min(remaining, Math.Min(bid.Amount, exchange.BtcBalance));
                if (amount <= 0) continue;

                orders.Add(new ExecutionOrder
                {
                    ExchangeId = exchange.Id,
                    Side = OrderSide.Sell,
                    Amount = amount,
                    Price = bid.Price
                });
                exchange.BtcBalance -= amount;
                remaining -= amount;
            }

            return BuildPlan(orders, btcAmount, remaining);
        }

        private static ExecutionPlan BuildPlan(List<ExecutionOrder> orders, decimal requested, decimal remaining)
        {
            decimal totalBtc = requested - remaining;
            decimal totalEur = orders.Sum(o => o.Total);
            decimal avgPrice = totalBtc > 0 ? totalEur / totalBtc : 0;

            return new ExecutionPlan
            {
                Orders = orders,
                TotalBtc = totalBtc,
                TotalEur = totalEur,
                AveragePrice = avgPrice,
                Status = GetFillStatus(totalBtc, remaining)
            };
        }

        private static FillStatus GetFillStatus(decimal totalBtc, decimal remaining)
        {
            if (totalBtc == 0)
            {
                return FillStatus.NotFilled;
            }

            if (remaining <= 0)
            {
                return FillStatus.FullyFilled;
            }

            return FillStatus.PartiallyFilled;
        }
    }
}
