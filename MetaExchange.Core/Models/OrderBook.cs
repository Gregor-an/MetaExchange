namespace MetaExchange.Core.Models
{
    public record OrderBook(
        DateTime AcqTime,
        List<OrderEntry> Bids,
        List<OrderEntry> Asks
    );
}
