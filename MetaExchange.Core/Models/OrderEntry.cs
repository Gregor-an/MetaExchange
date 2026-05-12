namespace MetaExchange.Core.Models
{
    /// <summary>
    /// Represents an order book entry containing a single order.
    /// </summary>
    public sealed record OrderEntry
    {
        /// <summary>
        /// Order data for this entry.
        /// </summary>
        public required Order Order { get; init; }
    }
}
