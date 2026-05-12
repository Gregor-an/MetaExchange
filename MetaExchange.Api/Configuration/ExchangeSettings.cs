namespace MetaExchange.Api.Configuration
{
    /// <summary>
    /// Defines application settings for exchange data and balances.
    /// </summary>
    public sealed class ExchangeSettings
    {
        /// <summary>
        /// Path to the order books data file.
        /// </summary>
        public required string DataFile { get; init; }

        /// <summary>
        /// Exchanges with their balance constraints.
        /// </summary>
        public List<ExchangeConfig> Exchanges { get; init; } = [];
    }
}
