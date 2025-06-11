using Nop.Core.Configuration;

namespace Nop.Plugin.DiscountRules.OrderHistory;

public class DiscountRulesOrderHistorySettings : ISettings
{
    public bool Enable { get; set; }
    public int MinOrderCount { get; set; } = 3;
    public decimal DiscountPercentage { get; set; } = 10;
}