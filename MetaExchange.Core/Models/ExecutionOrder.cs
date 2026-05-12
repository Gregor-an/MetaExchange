namespace MetaExchange.Core.Models
{
    public sealed record ExecutionOrder(
        string ExchangeId,
        OrderSide Side,
        decimal Amount,
        decimal Price,
        decimal Total
    );
}
