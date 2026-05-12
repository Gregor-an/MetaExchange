namespace MetaExchange.Core.Models
{
    /// <summary>
    /// Represents the best execution plan produced for a requested BTC order
    /// </summary>
    public sealed record ExecutionPlan
    {
        /// <summary>
        /// Ordered list of individual orders to execute across exchanges.
        /// </summary>
        public required IReadOnlyList<ExecutionOrder> Orders { get; init; }

        /// <summary>
        /// Total BTC amount that will be bought or sold.
        /// </summary>
        public decimal TotalBtc { get; init; }

        /// <summary>
        /// Total EUR amount spent for buy orders or received for sell orders.
        /// </summary>
        public decimal TotalEur { get; init; }

        /// <summary>
        /// Weighted average price in EUR per BTC.
        /// </summary>
        public decimal AveragePrice { get; init; }

        /// <summary>
        /// Indicates whether the requested BTC amount was fully, partially, or not filled.
        /// </summary>
        public FillStatus Status { get; init; }
    }
}
