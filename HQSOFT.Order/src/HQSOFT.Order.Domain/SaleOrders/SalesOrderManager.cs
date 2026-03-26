using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace HQSOFT.Order.SaleOrders;

public class SalesOrderManager : DomainService
{
    private readonly ISalesOrderRepository _salesOrderRepository;

    public SalesOrderManager(ISalesOrderRepository salesOrderRepository)
    {
        _salesOrderRepository = salesOrderRepository;
    }

    public async Task<SalesOrder> CreateAsync(DateTime orderDate)
    {
        var orderNumber = await GenerateUniqueOrderNumberAsync();
        var order = new SalesOrder(GuidGenerator.Create(), orderNumber, orderDate, ESaleOrderStatus.Draft);
        return await _salesOrderRepository.InsertAsync(order);
    }

    public async Task<SalesOrderLine> AddLineAsync(
        SalesOrder order,
        Guid productId,
        string productName,
        string productCode,
        int quantity,
        decimal unitPriceAmount,
        string currency = "VND")
    {
        Check.NotNull(order, nameof(order));

        if (order.Status != ESaleOrderStatus.Draft)
            throw new BusinessException("SaleOrder:CanOnlyAddLineWhenDraft")
                .WithData("CurrentStatus", order.Status.ToString());

        var unitPrice = new Money(unitPriceAmount, currency);
        var line = order.AddLine(GuidGenerator.Create(), productId, productName, productCode, unitPrice, quantity);
        return line;
    }

    public Task ConfirmAsync(SalesOrder order)
    {
        Check.NotNull(order, nameof(order));

        if (order.Status != ESaleOrderStatus.Draft)
            throw new BusinessException("SaleOrder:CanOnlyConfirmDraft")
                .WithData("CurrentStatus", order.Status.ToString());

        if (!order.OrderLines.Any())
            throw new BusinessException("SaleOrder:CannotConfirmEmptyOrder");

        order.SetStatus(ESaleOrderStatus.Confirmed);
        return Task.CompletedTask;
    }

    public Task CancelAsync(SalesOrder order)
    {
        Check.NotNull(order, nameof(order));

        if (order.Status == ESaleOrderStatus.Cancelled)
            throw new BusinessException("SaleOrder:AlreadyCancelled")
                .WithData("OrderNumber", order.OrderNumber);

        order.SetStatus(ESaleOrderStatus.Cancelled);
        return Task.CompletedTask;
    }



    private async Task<string> GenerateUniqueOrderNumberAsync()
    {
        var prefix = "SO";
        var dateStr = DateTime.UtcNow.ToString("yyyyMMdd");
        string orderNumber;
        int counter = 1;

        do
        {
            orderNumber = $"{prefix}-{dateStr}-{counter:D3}";
            counter++;
        }
        while (await _salesOrderRepository.ExistsOrderNumberAsync(orderNumber));

        return orderNumber;
    }
}
