namespace MetaExchange.Core.Models
{
    /// <summary>
    /// Indicates how much of the requested BTC amount was filled.
    /// </summary>
    public enum FillStatus
    {
        /// <summary>The full requested amount was executed.</summary>
        FullyFilled,
        /// <summary>Only part of the requested amount was executed.</summary>
        PartiallyFilled,
        /// <summary>No amount was executed (insufficient liquidity or balance).</summary>
        NotFilled
    }
}
