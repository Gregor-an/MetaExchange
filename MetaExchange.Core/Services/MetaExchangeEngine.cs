using MetaExchange.Core.Models;

namespace MetaExchange.Core.Services
{
    public static class MetaExchangeEngine
    {
        public static ExecutionPlan BuildExecutionPlan(IReadOnlyCollection<Exchange> exchanges, OrderSide side, decimal btcAmount)
        {
            if (btcAmount <= 0)
            {
                throw new ArgumentException("BTC amount must be greater than zero.", nameof(btcAmount));
            }

            return side switch
            {
                OrderSide.Buy => Buy(exchanges, btcAmount),
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
                if (remaining <= 0)
                {
                    break;
                }

                decimal maxByOrderBook = ask.Amount;
                decimal maxByBalance = exchange.EurBalance / ask.Price;
                decimal amount = Math.Min(remaining, Math.Min(maxByOrderBook, maxByBalance));

                if (amount <= 0)
                {
                    continue;
                }

                orders.Add(new ExecutionOrder(
                    exchange.Id,
                    OrderSide.Buy,
                    amount,
                    ask.Price,
                    amount * ask.Price));

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
                if (remaining <= 0)
                {
                    break;
                }

                decimal maxByOrderBook = bid.Amount;
                decimal maxByBalance = exchange.BtcBalance;
                decimal amount = Math.Min(remaining, Math.Min(maxByOrderBook, maxByBalance));

                if (amount <= 0)
                {
                    continue;
                }

                orders.Add(
                    new ExecutionOrder
                    (
                        exchange.Id,
                        OrderSide.Sell,
                        amount,
                        bid.Price,
                        amount * bid.Price
                    )
                );

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

            FillStatus status =
                totalBtc == 0 ? FillStatus.NotFilled :
                remaining <= 0 ? FillStatus.FullyFilled :
                FillStatus.PartiallyFilled;

            return new ExecutionPlan(orders, totalBtc, totalEur, avgPrice, status);
        }
    }
}