using System;
using System.Collections.Generic;

namespace HQSOFT.Order.SaleOrders;

public class SalesOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public ESaleOrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "VND";
    public List<SalesOrderLineDto> OrderLines { get; set; } = [];
}
