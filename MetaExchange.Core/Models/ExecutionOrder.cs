namespace MetaExchange.Core.Models
{
    /// <summary>
    /// Represents a single order to be executed on a specific exchange.
    /// </summary>
    public sealed record ExecutionOrder
    {
        /// <summary>
        /// Identifier of the exchange where the order should be placed.
        /// </summary>
        public required string ExchangeId { get; init; }

        /// <summary>
        /// Side of the order.
        /// </summary>
        public OrderSide Side { get; init; }

        /// <summary>
        /// Amount of BTC to buy or sell.
        /// </summary>
        public decimal Amount { get; init; }

        /// <summary>
        /// Price per BTC in EUR.
        /// </summary>
        public decimal Price { get; init; }

        /// <summary>
        /// Total EUR value of this order.
        /// </summary>
        public decimal Total => Amount * Price;
    }
}
