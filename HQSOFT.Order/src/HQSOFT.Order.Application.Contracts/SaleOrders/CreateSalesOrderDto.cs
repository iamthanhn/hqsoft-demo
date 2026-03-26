using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HQSOFT.Order.SaleOrders;

public class CreateSalesOrderDto
{
    [Required]
    public DateTime OrderDate { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateSalesOrderLineDto> Lines { get; set; } = [];
}
