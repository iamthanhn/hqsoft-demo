using System;

namespace HQSOFT.Order.SaleOrders;

public class SalesOrderLineDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPriceAmount { get; set; }
    public decimal LineTotalAmount { get; set; }
    public string Currency { get; set; } = "VND";
}
