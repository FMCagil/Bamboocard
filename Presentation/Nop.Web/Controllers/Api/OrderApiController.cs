using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Web.Models.Order;

namespace Nop.Web.Controllers.Api;

[Route("api/orders")]
[ApiController]
public class OrderApiController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICustomerService _customerService;

    public OrderApiController(IOrderService orderService, ICustomerService customerService)
    {
        _orderService = orderService;
        _customerService = customerService;
    }

    [HttpGet("by-email")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<IEnumerable<OrderSummaryModel>>> GetOrdersByEmailAsync([FromQuery] string email)
    {
        if (User.Identity != null)
        {
            var currentUserEmail = User.Identity.Name;

            // Sadece kendi hesabına erişsin (isteğe bağlı, security için önerilir)
            if (!string.Equals(currentUserEmail, email, StringComparison.OrdinalIgnoreCase))
                return Forbid();
        }

        var customer = await _customerService.GetCustomerByEmailAsync(email);
        if (customer == null)
            return NotFound();

        var orders = await _orderService.SearchOrdersAsync(customerId: customer.Id);

        var result = orders.Select(x => new OrderSummaryModel
        {
            OrderId = x.Id,
            TotalAmount = x.OrderTotal,
            OrderDate = x.CreatedOnUtc
        });
        return Ok(result);
    }

}
