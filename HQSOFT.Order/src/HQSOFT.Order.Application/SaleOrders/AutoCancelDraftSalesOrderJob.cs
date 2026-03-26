using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;

namespace HQSOFT.Order.SaleOrders;

public class AutoCancelDraftSalesOrderJob : AsyncBackgroundJob<AutoCancelDraftSalesOrderArgs>
{
    private readonly ISalesOrderRepository _salesOrderRepository;
    private readonly SalesOrderManager _salesOrderManager;

    public AutoCancelDraftSalesOrderJob(
        ISalesOrderRepository salesOrderRepository,
        SalesOrderManager salesOrderManager)
    {
        _salesOrderRepository = salesOrderRepository;
        _salesOrderManager = salesOrderManager;
    }

    public override async Task ExecuteAsync(AutoCancelDraftSalesOrderArgs args)
    {
        var order = await _salesOrderRepository.FindWithLinesAsync(args.SalesOrderId);
        if (order == null || order.Status != ESaleOrderStatus.Draft)
        {
            return;
        }

        await _salesOrderManager.CancelAsync(order);
        await _salesOrderRepository.UpdateAsync(order, autoSave: true);
    }
}
