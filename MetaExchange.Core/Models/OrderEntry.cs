namespace MetaExchange.Core.Models
{
    /// <summary>
    /// Wraps a single <see cref="Order"/> as an entry in the order book.
    /// </summary>
    public record OrderEntry(Order Order);
}
