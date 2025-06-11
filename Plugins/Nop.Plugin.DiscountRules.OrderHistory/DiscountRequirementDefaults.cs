namespace Nop.Plugin.DiscountRules.OrderHistory
{
    /// <summary>
    /// Represents defaults for the order history discount requirement rule
    /// </summary>
    public static class DiscountRequirementDefaults
    {
        public static string SystemName => "DiscountRequirement.OrderHistory";

        public static string SettingsKeyIsEnabled => "DiscountRequirement.OrderHistory.IsEnabled-{0}";
        public static string SettingsKeyMinOrderCount => "DiscountRequirement.OrderHistory.MinOrderCount-{0}";
        public static string SettingsKeyDiscountPercentage => "DiscountRequirement.OrderHistory.DiscountPercentage-{0}";

        public static string SettingsKey => "DiscountRequirement.OrderHistory-{0}";
        
        public static string ConfigurationRouteName => "Plugin.DiscountRules.OrderHistory.Configure";
    }
}

