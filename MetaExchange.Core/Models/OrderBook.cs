namespace MetaExchange.Core.Models
{
    /// <summary>
    /// Represents a snapshot of an exchange order book at a given point in time.
    /// </summary>
    public record OrderBook(
        /// <summary>Time when the snapshot was acquired.</summary>
        DateTime AcqTime,
        /// <summary>Buy orders (bids), sorted by price descending.</summary>
        List<OrderEntry> Bids,
        /// <summary>Sell orders (asks), sorted by price ascending.</summary>
        List<OrderEntry> Asks
    );
}
