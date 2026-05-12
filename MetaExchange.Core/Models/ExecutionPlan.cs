namespace MetaExchange.Core.Models
{
    public record ExecutionPlan(
        List<ExecutionOrder> Orders,
        decimal TotalBtc,
        decimal TotalEur,
        decimal AveragePrice,
        FillStatus Status
    );
}
