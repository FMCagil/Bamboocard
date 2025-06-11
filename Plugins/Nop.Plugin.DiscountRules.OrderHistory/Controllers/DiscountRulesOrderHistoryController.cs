using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Core.Domain.Discounts;
using Nop.Plugin.DiscountRules.OrderHistory.Models;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.DiscountRules.OrderHistory.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    [AutoValidateAntiforgeryToken]
    public partial class DiscountRulesOrderHistoryController : BasePluginController
    {
        #region Fields

        protected readonly ISettingService _settingService;
        protected readonly INotificationService _notificationService;
        protected readonly ILocalizationService _localizationService;
        protected readonly IDiscountService _discountService;
        protected readonly DiscountRulesOrderHistorySettings _discountRulesOrderHistorySettings;

        #endregion

        #region Ctor

        public DiscountRulesOrderHistoryController(ISettingService settingService,
            INotificationService notificationService,
            ILocalizationService localizationService, 
            IDiscountService discountService, 
            DiscountRulesOrderHistorySettings discountRulesOrderHistorySettings)
        {
            _settingService = settingService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _discountService = discountService;
            _discountRulesOrderHistorySettings = discountRulesOrderHistorySettings;
        }

        #endregion
        
        #region Utilities

        /// <summary>
        /// Get errors message from model state
        /// </summary>
        /// <param name="modelState">Model state</param>
        /// <returns>Errors message</returns>
        protected IEnumerable<string> GetErrorsFromModelState(ModelStateDictionary modelState)
        {
            return ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
        }

        #endregion

        #region Methods

        [CheckPermission(StandardPermission.Promotions.DISCOUNTS_VIEW)]
        public async Task<IActionResult> Configure()
        {
         
            var model = new ConfigurationModel();


                model.IsEnabled = await _settingService.GetSettingByKeyAsync<bool>(
                    DiscountRequirementDefaults.SettingsKeyIsEnabled, defaultValue: _discountRulesOrderHistorySettings.Enable);

                model.MinOrderCount = await _settingService.GetSettingByKeyAsync<int>(
                    DiscountRequirementDefaults.SettingsKeyMinOrderCount, defaultValue: _discountRulesOrderHistorySettings.MinOrderCount);;

                model.DiscountPercentage = await _settingService.GetSettingByKeyAsync<decimal>(
                    DiscountRequirementDefaults.SettingsKeyDiscountPercentage, defaultValue: _discountRulesOrderHistorySettings.DiscountPercentage);;
            

            return View("~/Plugins/DiscountRules.OrderHistory/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [CheckPermission(StandardPermission.Promotions.DISCOUNTS_CREATE_EDIT_DELETE)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Errors = GetErrorsFromModelState(ModelState) });

            try
            {
                if (ModelState.IsValid)
                {

                    // İstediğin yeni ayarları kaydet
                    await _settingService.SetSettingAsync(
                        DiscountRequirementDefaults.SettingsKeyIsEnabled, 
                        model.IsEnabled);

                    
                    await _settingService.SetSettingAsync(
                        DiscountRequirementDefaults.SettingsKeyMinOrderCount, model.MinOrderCount);


                    await _settingService.SetSettingAsync(
                        DiscountRequirementDefaults.SettingsKeyDiscountPercentage,
                        model.DiscountPercentage);     


                    return Ok();
                }

                return Ok(new { Errors = GetErrorsFromModelState(ModelState) });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Errors = new[] { "An unexpected error occurred.", ex.Message } });
            }
        }


        #endregion
    }
}