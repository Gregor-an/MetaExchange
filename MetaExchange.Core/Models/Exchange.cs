namespace MetaExchange.Core.Models
{
    /// <summary>
    /// Represents a crypto exchange with its order book and available balances.
    /// </summary>
    public class Exchange
    {
        /// <summary>
        /// Unique identifier of the exchange.
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Available EUR balance on this exchange.
        /// </summary>
        public decimal EurBalance { get; set; }

        /// <summary>
        /// Available BTC balance on this exchange.
        /// </summary>
        public decimal BtcBalance { get; set; }

        /// <summary>
        /// Current order book snapshot for this exchange.
        /// </summary>
        public required OrderBook OrderBook { get; init; }
    }
}
