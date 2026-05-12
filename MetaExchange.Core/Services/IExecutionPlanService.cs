using MetaExchange.Core.Models;

namespace MetaExchange.Core.Services
{
    /// <summary>
    /// Defines the contract for building a best execution plan.
    /// across multiple exchanges.
    /// </summary>
    public interface IExecutionPlanService
    {
        /// <summary>
        /// Builds the best available execution plan for the specified order side and BTC amount.
        /// </summary>
        /// <param name="exchanges">Available exchanges with their order books and balances.</param>
        /// <param name="side">Whether to buy or sell BTC.</param>
        /// <param name="btcAmount">The amount of BTC to buy or sell.</param>
        /// <returns>An <see cref="ExecutionPlan"/> describing the orders to execute.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="btcAmount"/> is not positive.</exception>
        ExecutionPlan BuildExecutionPlan(IReadOnlyCollection<Exchange> exchanges, OrderSide side, decimal btcAmount);
    }
}
