using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Events;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.DiscountRules.OrderHistory;

public class OrderPlacedDiscountConsumer : IConsumer<OrderPlacedEvent>, IConsumer<ModelPreparedEvent<OrderTotalsModel>>
{
    private readonly IOrderService _orderService;
    private readonly ISettingService _settingService;
    private readonly ILocalizationService _localizationService;
    private readonly IWorkContext _workContext;
    private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IStoreContext _storeContext;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    private readonly IPriceFormatter _priceFormatter;

    public OrderPlacedDiscountConsumer(
        IOrderService orderService,
        ISettingService settingService,
        ILocalizationService localizationService,
        IWorkContext workContext,
        IShoppingCartModelFactory shoppingCartModelFactory,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        IOrderTotalCalculationService orderTotalCalculationService,
        IPriceFormatter priceFormatter)
    {
        _orderService = orderService;
        _settingService = settingService;
        _localizationService = localizationService;
        _workContext = workContext;
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _orderTotalCalculationService = orderTotalCalculationService;
        _priceFormatter = priceFormatter;
    }

    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        var order = eventMessage.Order;

        var isEnabled = await _settingService.GetSettingByKeyAsync<bool>(
            DiscountRequirementDefaults.SettingsKeyIsEnabled);
        var minOrderCount = await _settingService.GetSettingByKeyAsync<int>(
            DiscountRequirementDefaults.SettingsKeyMinOrderCount);
        var discountPercentage = await _settingService.GetSettingByKeyAsync<decimal>(
            DiscountRequirementDefaults.SettingsKeyDiscountPercentage);

        var orderStatuses = new List<int> { (int)OrderStatus.Complete };
        var paymentStatus = new List<int>
        {
            (int)PaymentStatus.Paid, (int)PaymentStatus.PartiallyRefunded,
            (int)PaymentStatus.Refunded, (int)PaymentStatus.Voided
        };

        if (isEnabled && minOrderCount > 0)
        {
            // Sipariş notlarında bu otomatik indirim zaten var mı kontrolü
            var existingNotes = await _orderService.GetOrderNotesByOrderIdAsync(order.Id, displayToCustomer: false);

            // Konvansiyon: Benzersiz metin (plugin adı, anahtar vs.) tutun ve ona göre kontrol yapın
            string automaticDiscountKey = "[Nop.Plugin.DiscountRules.OrderHistory]";

            bool alreadyApplied = existingNotes.Any(n => n.Note != null && n.Note.Contains(automaticDiscountKey));

            if (alreadyApplied)
                return; // Bu indirim zaten uygulanmış, tekrar uygulama.

            
            var previousOrders = await _orderService.SearchOrdersAsync(
                customerId: order.CustomerId,
                osIds: orderStatuses,
                psIds: paymentStatus);

            if (previousOrders.Count() >= minOrderCount)
            {
                decimal discountAmount = order.OrderTotal * (discountPercentage / 100m);

                if (order.OrderTotal > discountAmount)
                {
                    order.OrderDiscount = discountAmount;
                    order.OrderTotal -= discountAmount;

                    await _orderService.UpdateOrderAsync(order);

                    // Localization note
                    string localizedNote = await _localizationService.GetResourceAsync(
                        "Plugins.Order.AutomaticDiscountOrderNote",
                        languageId: order.CustomerLanguageId // customer language
                    );

                    if (string.IsNullOrEmpty(localizedNote))
                    {
                        // if there is no localized note, use default one
                        localizedNote = "%{0} applied automatic discount. ({1})";
                    }

                    string formattedNote =
                        $"{automaticDiscountKey} " +
                        string.Format(localizedNote, discountPercentage, discountAmount.ToString("C2"));

                    var note = new OrderNote
                    {
                        OrderId = order.Id,
                        Note = formattedNote,
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    await _orderService.InsertOrderNoteAsync(note);

                }
            }
        }
    }

    public async Task HandleEventAsync(ModelPreparedEvent<OrderTotalsModel> eventMessage)
    {
        var model = eventMessage.Model;
        var customer = await _workContext.GetCurrentCustomerAsync();

        var isEnabled =
            await _settingService.GetSettingByKeyAsync<bool>("DiscountRequirementDefaults.SettingsKeyIsEnabled");
        var minOrderCount =
            await _settingService.GetSettingByKeyAsync<int>("DiscountRequirementDefaults.SettingsKeyMinOrderCount");
        var discountPercentage =
            await _settingService.GetSettingByKeyAsync<decimal>(
                "DiscountRequirementDefaults.SettingsKeyDiscountPercentage");

        var orderStatuses = new List<int> { (int)OrderStatus.Complete };
        var paymentStatus = new List<int>
        {
            (int)PaymentStatus.Paid, (int)PaymentStatus.PartiallyRefunded,
            (int)PaymentStatus.Refunded, (int)PaymentStatus.Voided
        };

        var previousOrders = await _orderService.SearchOrdersAsync(
            customerId: customer.Id,
            osIds: orderStatuses,
            psIds: paymentStatus);

        if (isEnabled && previousOrders.Count() >= minOrderCount)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart,
                store.Id);
            var orderTotalCalculated =
                (await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart)).shoppingCartTotal ?? 0;

            decimal discountAmount = orderTotalCalculated * (discountPercentage / 100m);

            if (orderTotalCalculated > discountAmount)
            {
                model.OrderTotalDiscount = await _priceFormatter.FormatPriceAsync(discountAmount, true, false);
                model.OrderTotal =
                    await _priceFormatter.FormatPriceAsync(orderTotalCalculated - discountAmount, true, false);
            }
        }
    }
}