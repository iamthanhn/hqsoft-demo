using System;
using System.Linq;
using System.Threading.Tasks;
using HQSOFT.Inventory.Integration;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace HQSOFT.Order.SaleOrders;

public class SalesOrderAppService_Tests : OrderApplicationTestBase<OrderApplicationTestModule>
{
    private readonly ISalesOrderAppService _appService;
    private readonly FakeInventoryIntegrationService _inventoryService;
    private readonly RecordingDistributedEventBus _eventBus;
    private readonly RecordingBackgroundJobManager _backgroundJobManager;

    public SalesOrderAppService_Tests()
    {
        _appService = GetRequiredService<ISalesOrderAppService>();
        _inventoryService = GetRequiredService<FakeInventoryIntegrationService>();
        _eventBus = GetRequiredService<RecordingDistributedEventBus>();
        _backgroundJobManager = GetRequiredService<RecordingBackgroundJobManager>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Draft_And_Enqueue_AutoCancel_Job()
    {
        var productId = Guid.NewGuid();
        _inventoryService.SetStock(productId, isAvailable: true, availableQuantity: 10, productCode: "P-001", productName: "Tour", reserveResult: true);

        var result = await _appService.CreateAsync(new CreateSalesOrderDto
        {
            OrderDate = new DateTime(2026, 3, 26),
            Lines =
            [
                new CreateSalesOrderLineDto
                {
                    ProductId = productId,
                    ProductName = "Tour",
                    ProductCode = "P-001",
                    Quantity = 2,
                    UnitPriceAmount = 1500000,
                    Currency = "VND"
                }
            ]
        });

        result.Status.ShouldBe(ESaleOrderStatus.Draft);
        result.OrderLines.Count.ShouldBe(1);
        _backgroundJobManager.EnqueuedJobs.Any(x => x.Args is AutoCancelDraftSalesOrderArgs args && args.SalesOrderId == result.Id)
            .ShouldBeTrue();
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Stock_Is_Not_Available()
    {
        var productId = Guid.NewGuid();
        _inventoryService.SetStock(productId, isAvailable: false, availableQuantity: 0, productCode: "P-002", productName: "Combo", reserveResult: false);

        await Should.ThrowAsync<BusinessException>(async () =>
            await _appService.CreateAsync(new CreateSalesOrderDto
            {
                OrderDate = DateTime.Today,
                Lines =
                [
                    new CreateSalesOrderLineDto
                    {
                        ProductId = productId,
                        ProductName = "Combo",
                        ProductCode = "P-002",
                        Quantity = 1,
                        UnitPriceAmount = 500000,
                        Currency = "VND"
                    }
                ]
            }));
    }

    [Fact]
    public async Task ConfirmAsync_Should_Reserve_Stock_And_Publish_Event()
    {
        var productId = Guid.NewGuid();
        _inventoryService.SetStock(productId, isAvailable: true, availableQuantity: 10, productCode: "P-003", productName: "Hotel", reserveResult: true);

        var order = await _appService.CreateAsync(new CreateSalesOrderDto
        {
            OrderDate = DateTime.Today,
            Lines =
            [
                new CreateSalesOrderLineDto
                {
                    ProductId = productId,
                    ProductName = "Hotel",
                    ProductCode = "P-003",
                    Quantity = 3,
                    UnitPriceAmount = 700000,
                    Currency = "VND"
                }
            ]
        });

        var confirmed = await _appService.ConfirmAsync(order.Id);

        confirmed.Status.ShouldBe(ESaleOrderStatus.Confirmed);
        _inventoryService.ReserveCalls.Count.ShouldBe(1);
        _eventBus.PublishedEvents.Any(x => x is SalesOrderConfirmedEto eto && eto.OrderId == order.Id)
            .ShouldBeTrue();
    }

    [Fact]
    public async Task CancelAsync_Should_Set_Status_To_Cancelled()
    {
        var productId = Guid.NewGuid();
        _inventoryService.SetStock(productId, isAvailable: true, availableQuantity: 10, productCode: "P-004", productName: "Flight", reserveResult: true);

        var order = await _appService.CreateAsync(new CreateSalesOrderDto
        {
            OrderDate = DateTime.Today,
            Lines =
            [
                new CreateSalesOrderLineDto
                {
                    ProductId = productId,
                    ProductName = "Flight",
                    ProductCode = "P-004",
                    Quantity = 1,
                    UnitPriceAmount = 2000000,
                    Currency = "VND"
                }
            ]
        });

        var cancelled = await _appService.CancelAsync(order.Id);

        cancelled.Status.ShouldBe(ESaleOrderStatus.Cancelled);
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Paged_Result()
    {
        var productId = Guid.NewGuid();
        _inventoryService.SetStock(productId, isAvailable: true, availableQuantity: 50, productCode: "P-005", productName: "Visa", reserveResult: true);

        await _appService.CreateAsync(new CreateSalesOrderDto
        {
            OrderDate = DateTime.Today,
            Lines =
            [
                new CreateSalesOrderLineDto
                {
                    ProductId = productId,
                    ProductName = "Visa",
                    ProductCode = "P-005",
                    Quantity = 1,
                    UnitPriceAmount = 300000,
                    Currency = "VND"
                }
            ]
        });

        var result = await _appService.GetListAsync(new GetSalesOrdersInput
        {
            MaxResultCount = 10,
            SkipCount = 0
        });

        result.ShouldBeOfType<PagedResultDto<SalesOrderDto>>();
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldContain(x => x.OrderLines.Any());
    }
}
