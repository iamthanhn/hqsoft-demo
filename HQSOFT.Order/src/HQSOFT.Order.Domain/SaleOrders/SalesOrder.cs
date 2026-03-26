using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace HQSOFT.Order.SaleOrders;

public class SalesOrder : FullAuditedAggregateRoot<Guid>
{
    public string OrderNumber { get; private set; }

    public DateTime OrderDate { get; private set; }

    public ESaleOrderStatus Status { get; private set; }

    public virtual ICollection<SalesOrderLine> OrderLines { get; private set; } = [];

    protected SalesOrder()
    {
    }

    internal SalesOrder(
        Guid id,
        string orderNumber,
        DateTime orderDate,
        ESaleOrderStatus status) : base(id)
    {
        OrderNumber = Check.NotNullOrWhiteSpace(orderNumber, nameof(orderNumber), SalesOrderConsts.MaxOrderNumber);
        OrderDate = orderDate;
        Status = status;
    }

    public SalesOrderLine AddLine(
        Guid id,
        Guid productId,
        string productName,
        string productCode,
        Money unitPrice,
        int quantity)
    {
        Check.NotNullOrWhiteSpace(productName, nameof(productName), SalesOrderConsts.MaxProductName);
        Check.NotNullOrWhiteSpace(productCode, nameof(productCode), SalesOrderConsts.MaxProductCode);
        Check.NotNull(unitPrice, nameof(unitPrice));

        var line = new SalesOrderLine(
            id,
            Id,
            productId,
            productName,
            productCode,
            unitPrice,
            quantity);

        OrderLines.Add(line);
        return line;
    }

    public void RemoveLine(Guid lineId)
    {
        var line = OrderLines.FirstOrDefault(l => l.Id == lineId);
        if (line == null)
            throw new BusinessException("SaleOrder:LineNotFound")
                .WithData("LineId", lineId.ToString());

        OrderLines.Remove(line);
    }

    public void SetStatus(ESaleOrderStatus status)
    {
        Status = status;
    }

    public Money GetTotal()
    {
        return OrderLines.Aggregate(
            new Money(0),
            (total, line) => total + line.LineTotal);
    }
}
