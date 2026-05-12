namespace MetaExchange.Core.Models
{
    /// <summary>
    /// Represents a single order in an order book.
    /// </summary>
    public sealed record Order
    {
        /// <summary>
        /// Optional order identifier.
        /// </summary>
        public string? Id { get; init; }

        /// <summary>
        /// Time when the order was placed.
        /// </summary>
        public DateTime Time { get; init; }

        /// <summary>
        /// Order side as provided by the source data, for example Buy or Sell.
        /// </summary>
        public required string Type { get; init; }

        /// <summary>
        /// Order kind as provided by the source data, for example Limit.
        /// </summary>
        public required string Kind { get; init; }

        /// <summary>
        /// Amount of BTC in this order.
        /// </summary>
        public decimal Amount { get; init; }

        /// <summary>
        /// Price per BTC in EUR.
        /// </summary>
        public decimal Price { get; init; }
    }
}
