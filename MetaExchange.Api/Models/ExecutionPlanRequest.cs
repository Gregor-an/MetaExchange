using MetaExchange.Core.Models;

namespace MetaExchange.Api.Models
{
    /// <summary>
    /// Represents a request for creating execution plan.
    /// </summary>
    public class ExecutionPlanRequest
    {
        /// <summary>
        /// Order side: Buy or Sell.
        /// </summary>
        public OrderSide Side { get; init; }

        /// <summary>
        /// Amount of BTC to buy or sell. Must be greater than zero.
        /// </summary>
        public decimal Amount { get; init; }
    }
}
