using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace HQSOFT.Order.SaleOrders;

public class SalesOrderLine : Entity<Guid>
{
    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public string ProductName { get; private set; }

    public string ProductCode { get; private set; }

    public Money UnitPrice { get; private set; }

    public Money LineTotal { get; private set; }

    public int Quantity { get; private set; }

    protected SalesOrderLine()
    {
    }

    internal SalesOrderLine(
        Guid id,
        Guid orderId,
        Guid productId,
        string productName,
        string productCode,
        Money unitPrice,
        int quantity) : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        ProductCode = productCode;
        UnitPrice = unitPrice;
        Quantity = quantity;
        LineTotal = unitPrice * quantity;
    }
}
