using System;
using Volo.Abp.Application.Dtos;

namespace HQSOFT.Order.SaleOrders;

public class GetSalesOrdersInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public ESaleOrderStatus? Status { get; set; }
    public DateTime? FromOrderDate { get; set; }
    public DateTime? ToOrderDate { get; set; }
}
