using System;
using System.ComponentModel.DataAnnotations;

namespace HQSOFT.Order.SaleOrders;

public class CreateSalesOrderLineDto
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [StringLength(SalesOrderConsts.MaxProductName)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [StringLength(SalesOrderConsts.MaxProductCode)]
    public string ProductCode { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal UnitPriceAmount { get; set; }

    [Required]
    [StringLength(8)]
    public string Currency { get; set; } = "VND";
}
