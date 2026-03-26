using System;
using System.Threading.Tasks;
using HQSOFT.Inventory.Integration;
using Shouldly;
using Xunit;

namespace HQSOFT.Order.SaleOrders;

public class AutoCancelDraftSalesOrderJob_Tests : OrderApplicationTestBase<OrderApplicationTestModule>
{
    private readonly AutoCancelDraftSalesOrderJob _job;
    private readonly ISalesOrderAppService _appService;
    private readonly ISalesOrderRepository _repository;
    private readonly FakeInventoryIntegrationService _inventoryService;

    public AutoCancelDraftSalesOrderJob_Tests()
    {
        _job = GetRequiredService<AutoCancelDraftSalesOrderJob>();
        _appService = GetRequiredService<ISalesOrderAppService>();
        _repository = GetRequiredService<ISalesOrderRepository>();
        _inventoryService = GetRequiredService<FakeInventoryIntegrationService>();
    }

    [Fact]
    public async Task ExecuteAsync_Should_Cancel_Draft_Order()
    {
        var productId = Guid.NewGuid();
        _inventoryService.SetStock(productId, true, 10, "P-100", "Draft Order", true);

        var order = await _appService.CreateAsync(new CreateSalesOrderDto
        {
            OrderDate = DateTime.Today,
            Lines =
            [
                new CreateSalesOrderLineDto
                {
                    ProductId = productId,
                    ProductName = "Draft Order",
                    ProductCode = "P-100",
                    Quantity = 1,
                    UnitPriceAmount = 1000,
                    Currency = "VND"
                }
            ]
        });

        await _job.ExecuteAsync(new AutoCancelDraftSalesOrderArgs { SalesOrderId = order.Id });

        var reloaded = await _repository.FindWithLinesAsync(order.Id);
        reloaded!.Status.ShouldBe(ESaleOrderStatus.Cancelled);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Not_Change_Confirmed_Order()
    {
        var productId = Guid.NewGuid();
        _inventoryService.SetStock(productId, true, 10, "P-101", "Confirmed Order", true);

        var order = await _appService.CreateAsync(new CreateSalesOrderDto
        {
            OrderDate = DateTime.Today,
            Lines =
            [
                new CreateSalesOrderLineDto
                {
                    ProductId = productId,
                    ProductName = "Confirmed Order",
                    ProductCode = "P-101",
                    Quantity = 1,
                    UnitPriceAmount = 1000,
                    Currency = "VND"
                }
            ]
        });

        await _appService.ConfirmAsync(order.Id);
        await _job.ExecuteAsync(new AutoCancelDraftSalesOrderArgs { SalesOrderId = order.Id });

        var reloaded = await _repository.FindWithLinesAsync(order.Id);
        reloaded!.Status.ShouldBe(ESaleOrderStatus.Confirmed);
    }
}
