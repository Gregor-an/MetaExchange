namespace MetaExchange.Core.Models
{
    /// <summary>
    /// Represents a single order in an order book.
    /// </summary>
    public record Order(
        /// <summary>Optional order identifier.</summary>
        string? Id,
        /// <summary>Time the order was placed.</summary>
        DateTime Time,
        /// <summary>Order type: "Buy" or "Sell".</summary>
        string Type,
        /// <summary>Order kind, e.g. "Limit".</summary>
        string Kind,
        /// <summary>Amount of BTC in this order.</summary>
        decimal Amount,
        /// <summary>Price per BTC in EUR.</summary>
        decimal Price
    );
}
