using FluentAssertions;
using MetaExchange.Core.Models;
using MetaExchange.Core.Services;

namespace MetaExchange.Tests
{
    public sealed class ExecutionPlanServiceTests
    {
        private readonly IExecutionPlanService _service = new ExecutionPlanService();

        // ---------------------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------------------

        private static Exchange CreateExchange(
            string id,
            decimal eurBalance,
            decimal btcBalance,
            List<OrderEntry>? asks = null,
            List<OrderEntry>? bids = null)
        {
            var orderBook = new OrderBook
            {
                AcqTime = DateTime.UtcNow,
                Asks = asks ?? [],
                Bids = bids ?? []
            };

            return new Exchange
            {
                Id = id,
                EurBalance = eurBalance,
                BtcBalance = btcBalance,
                OrderBook = orderBook
            };
        }

        private static OrderEntry Ask(decimal price, decimal amount) => new()
        {
            Order = new Order { Price = price, Amount = amount, Type = "Sell", Kind = "Limit" }
        };

        private static OrderEntry Bid(decimal price, decimal amount) => new()
        {
            Order = new Order { Price = price, Amount = amount, Type = "Buy", Kind = "Limit" }
        };

        // ---------------------------------------------------------------------------
        // Validation
        // ---------------------------------------------------------------------------

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void BuildExecutionPlan_WhenAmountIsZeroOrNegative_ThrowsArgumentException(decimal amount)
        {
            var exchanges = new List<Exchange>();

            Action act = () => _service.BuildExecutionPlan(exchanges, OrderSide.Buy, amount);

            act.Should().Throw<ArgumentException>();
        }

        // ---------------------------------------------------------------------------
        // BUY
        // ---------------------------------------------------------------------------

        [Fact]
        public void Buy_WhenEnoughLiquidity_ReturnsFullyFilled()
        {
            var exchanges = new List<Exchange>
            {
                CreateExchange("E1", eurBalance: 10_000, btcBalance: 0, asks: [Ask(100, 5)])
            };

            var plan = _service.BuildExecutionPlan(exchanges, OrderSide.Buy, 2m);

            plan.Status.Should().Be(FillStatus.FullyFilled);
            plan.TotalBtc.Should().Be(2m);
            plan.TotalEur.Should().Be(200m);
        }

        [Fact]
        public void Buy_UsesCheapestAsksFirst()
        {
            var exchanges = new List<Exchange>
            {
                CreateExchange("E1", eurBalance: 10_000, btcBalance: 0, asks: [Ask(200, 1)]),
                CreateExchange("E2", eurBalance: 10_000, btcBalance: 0, asks: [Ask(100, 1)])
            };

            var plan = _service.BuildExecutionPlan(exchanges, OrderSide.Buy, 2m);

            plan.Orders[0].Price.Should().Be(100m);
            plan.Orders[1].Price.Should().Be(200m);
            plan.AveragePrice.Should().Be(150m);
            plan.TotalEur.Should().Be(300m);
        }

        [Fact]
        public void Buy_RespectsEurBalance()
        {
            // EUR balance only allows buying 1 BTC at price 100
            var exchanges = new List<Exchange>
            {
                CreateExchange("E1", eurBalance: 100, btcBalance: 0, asks: [Ask(100, 5)])
            };

            var plan = _service.BuildExecutionPlan(exchanges, OrderSide.Buy, 5m);

            plan.Status.Should().Be(FillStatus.PartiallyFilled);
            plan.TotalBtc.Should().Be(1m);
            plan.Orders.Should().ContainSingle();
            plan.Orders[0].Amount.Should().Be(1m);
        }

        [Fact]
        public void Buy_WhenNotEnoughLiquidity_ReturnsPartiallyFilled()
        {
            var exchanges = new List<Exchange>
            {
                CreateExchange("E1", eurBalance: 10_000, btcBalance: 0, asks: [Ask(100, 1)])
            };

            var plan = _service.BuildExecutionPlan(exchanges, OrderSide.Buy, 5m);

            plan.Status.Should().Be(FillStatus.PartiallyFilled);
            plan.TotalBtc.Should().Be(1m);
        }

        [Fact]
        public void Buy_WhenNoAsks_ReturnsNotFilled()
        {
            var exchanges = new List<Exchange>
            {
                CreateExchange("E1", eurBalance: 10_000, btcBalance: 0, asks: [])
            };

            var plan = _service.BuildExecutionPlan(exchanges, OrderSide.Buy, 2m);

            plan.Status.Should().Be(FillStatus.NotFilled);
            plan.TotalBtc.Should().Be(0m);
            plan.Orders.Should().BeEmpty();
        }

        [Fact]
        public void Buy_WhenAvailableAmountEqualsRequested_ReturnsFullyFilled()
        {
            var exchanges = new List<Exchange>
            {
                CreateExchange("E1", eurBalance: 200, btcBalance: 0, asks: [Ask(100, 2)])
            };

            var plan = _service.BuildExecutionPlan(exchanges, OrderSide.Buy, 2m);

            plan.Status.Should().Be(FillStatus.FullyFilled);
            plan.TotalBtc.Should().Be(2m);
        }

        // ---------------------------------------------------------------------------
        // SELL
        // ---------------------------------------------------------------------------

        [Fact]
        public void Sell_WhenEnoughLiquidity_ReturnsFullyFilled()
        {
            var exchanges = new List<Exchange>
            {
                CreateExchange("E1", eurBalance: 0, btcBalance: 10, bids: [Bid(100, 5)])
            };

            var plan = _service.BuildExecutionPlan(exchanges, OrderSide.Sell, 2m);

            plan.Status.Should().Be(FillStatus.FullyFilled);
            plan.TotalBtc.Should().Be(2m);
            plan.TotalEur.Should().Be(200m);
        }

        [Fact]
        public void Sell_UsesHighestBidsFirst()
        {
            var exchanges = new List<Exchange>
            {
                CreateExchange("E1", eurBalance: 0, btcBalance: 10, bids: [Bid(100, 1)]),
                CreateExchange("E2", eurBalance: 0, btcBalance: 10, bids: [Bid(200, 1)])
            };

            var plan = _service.BuildExecutionPlan(exchanges, OrderSide.Sell, 2m);

            plan.Orders[0].Price.Should().Be(200m);
            plan.Orders[1].Price.Should().Be(100m);
            plan.TotalEur.Should().Be(300m);
        }

        [Fact]
        public void Sell_RespectsBtcBalance()
        {
            // BTC balance only allows selling 1 BTC
            var exchanges = new List<Exchange>
            {
                CreateExchange("E1", eurBalance: 0, btcBalance: 1, bids: [Bid(100, 5)])
            };

            var plan = _service.BuildExecutionPlan(exchanges, OrderSide.Sell, 5m);

            plan.Status.Should().Be(FillStatus.PartiallyFilled);
            plan.TotalBtc.Should().Be(1m);
            plan.Orders.Should().ContainSingle();
            plan.Orders[0].Amount.Should().Be(1m);
        }

        [Fact]
        public void Sell_WhenNotEnoughLiquidity_ReturnsPartiallyFilled()
        {
            var exchanges = new List<Exchange>
            {
                CreateExchange("E1", eurBalance: 0, btcBalance: 10, bids: [Bid(100, 1)])
            };

            var plan = _service.BuildExecutionPlan(exchanges, OrderSide.Sell, 5m);

            plan.Status.Should().Be(FillStatus.PartiallyFilled);
            plan.TotalBtc.Should().Be(1m);
        }

        [Fact]
        public void Sell_WhenNoBids_ReturnsNotFilled()
        {
            var exchanges = new List<Exchange>
            {
                CreateExchange("E1", eurBalance: 0, btcBalance: 10, bids: [])
            };

            var plan = _service.BuildExecutionPlan(exchanges, OrderSide.Sell, 2m);

            plan.Status.Should().Be(FillStatus.NotFilled);
            plan.TotalBtc.Should().Be(0m);
            plan.Orders.Should().BeEmpty();
        }

        // ---------------------------------------------------------------------------
        // Edge cases
        // ---------------------------------------------------------------------------

        [Fact]
        public void BuildExecutionPlan_WhenExchangesAreEmpty_ReturnsNotFilled()
        {
            var plan = _service.BuildExecutionPlan([], OrderSide.Buy, 1m);

            plan.Status.Should().Be(FillStatus.NotFilled);
            plan.Orders.Should().BeEmpty();
        }

        [Fact]
        public void Buy_WhenOrdersHaveZeroPriceOrAmount_IgnoresInvalidOrders()
        {
            var exchanges = new List<Exchange>
            {
                CreateExchange("E1", eurBalance: 10_000, btcBalance: 0, asks:
                [
                    Ask(0, 5),    // zero price — ignored
                    Ask(100, 0),  // zero amount — ignored
                    Ask(100, 2)   // valid
                ])
            };

            var plan = _service.BuildExecutionPlan(exchanges, OrderSide.Buy, 2m);

            plan.Status.Should().Be(FillStatus.FullyFilled);
            plan.Orders.Should().ContainSingle();
        }
    }
}
