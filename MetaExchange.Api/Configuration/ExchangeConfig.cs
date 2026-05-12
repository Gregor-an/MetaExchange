namespace MetaExchange.Api.Configuration
{
    /// <summary>
    /// Represents a single exchange configuration entry with balance constraints.
    /// </summary>
    public sealed class ExchangeConfig
    {
        /// <summary>
        /// Unique identifier of the exchange.
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Available EUR balance on this exchange.
        /// </summary>
        public decimal EurBalance { get; init; }

        /// <summary>
        /// Available BTC balance on this exchange.
        /// </summary>
        public decimal BtcBalance { get; init; }
    }
}
