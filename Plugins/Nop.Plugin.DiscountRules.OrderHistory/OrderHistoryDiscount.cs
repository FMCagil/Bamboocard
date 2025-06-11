using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Plugins;

namespace Nop.Plugin.DiscountRules.OrderHistory;

    public partial class OrderHistoryDiscount : BasePlugin
    {
        #region Fields

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public OrderHistoryDiscount(IActionContextAccessor actionContextAccessor,
            IOrderService orderService,
            IWorkContext workContext, 
            IDiscountService discountService, 
            ILocalizationService localizationService, 
            IUrlHelperFactory urlHelperFactory, 
            IWebHelper webHelper, 
            ISettingService settingService)
        {
            _actionContextAccessor = actionContextAccessor;
            _orderService = orderService;
            _workContext = workContext;
            _discountService = discountService;
            _localizationService = localizationService;
            _urlHelperFactory = urlHelperFactory;
            _webHelper = webHelper;
            _settingService = settingService;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/DiscountRulesOrderHistory/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.DiscountRules.OrderHistory.Fields.MinOrderCount"] = "Minimum order count",
                ["Plugins.DiscountRules.OrderHistory.Fields.MinOrderCount.Hint"] = "Discount will be applied if the customer's total number of previous orders is equal to or greater than this value.",
                ["Plugins.DiscountRules.OrderHistory.Fields.MinOrderCount.Required"] = "Minimum order count is required",
                ["Plugins.DiscountRules.OrderHistory.Fields.DiscountId.Required"] = "Discount is required"
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //discount requirements
            var discountRequirements = (await _discountService.GetAllDiscountRequirementsAsync())
                .Where(discountRequirement => discountRequirement.DiscountRequirementRuleSystemName == DiscountRequirementDefaults.SystemName);
            foreach (var discountRequirement in discountRequirements)
            {
                await _discountService.DeleteDiscountRequirementAsync(discountRequirement, false);
            }

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.DiscountRules.OrderHistory");

            await base.UninstallAsync();
        }
        

        #endregion
        
    }
