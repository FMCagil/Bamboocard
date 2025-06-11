namespace Nop.Plugin.DiscountRules.OrderHistory.Models
{
    public class ConfigurationModel
    {
        public bool IsEnabled { get; set; }

        public int MinOrderCount { get; set; }

        public decimal DiscountPercentage { get; set; }
    }
}

