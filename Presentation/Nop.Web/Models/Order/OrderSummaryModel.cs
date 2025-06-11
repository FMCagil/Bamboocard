namespace Nop.Web.Models.Order;

public partial record OrderSummaryModel
{
    public int OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }

}