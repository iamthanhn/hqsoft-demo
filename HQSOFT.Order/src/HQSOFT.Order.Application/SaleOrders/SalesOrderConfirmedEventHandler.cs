using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace HQSOFT.Order.SaleOrders;

public class SalesOrderConfirmedEventHandler : IDistributedEventHandler<SalesOrderConfirmedEto>, ITransientDependency
{
    public Task HandleEventAsync(SalesOrderConfirmedEto eventData)
    {
        Console.WriteLine($"MOCK EMAIL: Sales order {eventData.OrderNumber} confirmed. Total: {eventData.TotalAmount} {eventData.Currency}");
        return Task.CompletedTask;
    }
}
