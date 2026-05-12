namespace MetaExchange.Core.Models
{
    /// <summary>
    /// Indicates how much of the requested BTC amount was filled.
    /// </summary>
    public enum FillStatus
    {
        /// <summary>
        /// No amount was executed because of insufficient liquidity or balance.
        /// </summary>
        NotFilled,

        /// <summary>
        /// Only part of the requested amount was executed.
        /// </summary>
        PartiallyFilled,

        /// <summary>
        /// The full requested amount was executed.
        /// </summary>
        FullyFilled
    }
}
