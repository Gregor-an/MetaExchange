namespace MetaExchange.Core.Models
{
    public class Exchange
    {
        public required string Id { get; init; }
        public decimal EurBalance { get; set; }
        public decimal BtcBalance { get; set; }
        public required OrderBook OrderBook { get; init; }
    }
}