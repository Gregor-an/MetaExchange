namespace MetaExchange.Core.Models
{
    /// <summary>
    /// Represents an exchange order book at a given point in time.
    /// </summary>
    public sealed record OrderBook
    {
        /// <summary>
        /// Time when the order book was acquired.
        /// </summary>
        public DateTime AcqTime { get; init; }

        /// <summary>
        /// Buy orders available in the order book.
        /// </summary>
        public required List<OrderEntry> Bids { get; init; }

        /// <summary>
        /// Sell orders available in the order book.
        /// </summary>
        public required List<OrderEntry> Asks { get; init; }
    }
}
