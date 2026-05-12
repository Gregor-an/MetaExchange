namespace MetaExchange.Core.Models
{
    /// <summary>
    /// Represents a single order to be executed on a specific exchange.
    /// </summary>
    public sealed record ExecutionOrder(
        /// <summary>Identifier of the exchange where the order should be placed.</summary>
        string ExchangeId,
        /// <summary>Side of the order.</summary>
        OrderSide Side,
        /// <summary>Amount of BTC to buy or sell.</summary>
        decimal Amount,
        /// <summary>Price per BTC in EUR.</summary>
        decimal Price,
        /// <summary>Total EUR value of this order (Amount × Price).</summary>
        decimal Total
    );
}
