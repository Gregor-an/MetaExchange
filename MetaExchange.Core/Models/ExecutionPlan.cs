namespace MetaExchange.Core.Models
{
    /// <summary>
    /// Represents the best execution plan produced by the algorithm.
    /// </summary>
    public record ExecutionPlan(
        /// <summary>Ordered list of individual orders to execute across exchanges.</summary>
        List<ExecutionOrder> Orders,
        /// <summary>Total BTC amount that will be bought or sold.</summary>
        decimal TotalBtc,
        /// <summary>Total EUR amount spent (buy) or received (sell).</summary>
        decimal TotalEur,
        /// <summary>Weighted average price in EUR per BTC.</summary>
        decimal AveragePrice,
        /// <summary>Indicates whether the requested amount was fully or partially filled.</summary>
        FillStatus Status
    );
}
