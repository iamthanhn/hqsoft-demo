using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HQSOFT.Inventory.Integration;
using HQSOFT.Order.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Linq;
using System.Linq.Dynamic.Core;

namespace HQSOFT.Order.SaleOrders;

[Authorize(OrderPermissions.SalesOrders.Default)]
public class SalesOrderAppService : OrderAppService, ISalesOrderAppService
{
    private readonly ISalesOrderRepository _salesOrderRepository;
    private readonly SalesOrderManager _salesOrderManager;
    private readonly IInventoryIntegrationService _inventoryIntegrationService;
    private readonly IBackgroundJobManager _backgroundJobManager;
    private readonly IDistributedEventBus _distributedEventBus;

    public SalesOrderAppService(
        ISalesOrderRepository salesOrderRepository,
        SalesOrderManager salesOrderManager,
        IInventoryIntegrationService inventoryIntegrationService,
        IBackgroundJobManager backgroundJobManager,
        IDistributedEventBus distributedEventBus)
    {
        _salesOrderRepository = salesOrderRepository;
        _salesOrderManager = salesOrderManager;
        _inventoryIntegrationService = inventoryIntegrationService;
        _backgroundJobManager = backgroundJobManager;
        _distributedEventBus = distributedEventBus;
    }

    public async Task<PagedResultDto<SalesOrderDto>> GetListAsync(GetSalesOrdersInput input)
    {
        var queryable = await _salesOrderRepository.GetQueryableAsync();

        var query = queryable
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(), x => x.OrderNumber.Contains(input.Filter!))
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status)
            .WhereIf(input.FromOrderDate.HasValue, x => x.OrderDate >= input.FromOrderDate!.Value)
            .WhereIf(input.ToOrderDate.HasValue, x => x.OrderDate <= input.ToOrderDate!.Value);

        var totalCount = await AsyncExecuter.CountAsync(query);
        var sorting = input.Sorting.IsNullOrWhiteSpace() ? nameof(SalesOrder.OrderDate) + " desc" : input.Sorting!;
        var items = await AsyncExecuter.ToListAsync(query.OrderBy(sorting).PageBy(input));

        var loadedItems = new List<SalesOrder>(items.Count);
        foreach (var item in items)
        {
            var loaded = await _salesOrderRepository.FindWithLinesAsync(item.Id);
            if (loaded != null)
            {
                loadedItems.Add(loaded);
            }
        }

        return new PagedResultDto<SalesOrderDto>(
            totalCount,
            ObjectMapper.Map<List<SalesOrder>, List<SalesOrderDto>>(loadedItems));
    }

    public async Task<SalesOrderDto> GetAsync(Guid id)
    {
        var order = await _salesOrderRepository.FindWithLinesAsync(id)
            ?? throw new EntityNotFoundException(typeof(SalesOrder), id);

        return ObjectMapper.Map<SalesOrder, SalesOrderDto>(order);
    }

    [Authorize(OrderPermissions.SalesOrders.Create)]
    public async Task<SalesOrderDto> CreateAsync(CreateSalesOrderDto input)
    {
        var order = await _salesOrderManager.CreateAsync(input.OrderDate);

        foreach (var line in input.Lines)
        {
            var stock = await _inventoryIntegrationService.CheckStockAsync(line.ProductId, line.Quantity);
            if (!stock.IsAvailable)
            {
                var productCode = string.IsNullOrWhiteSpace(stock.ProductCode) ? line.ProductCode : stock.ProductCode;
                throw new BusinessException(OrderDomainErrorCodes.InsufficientStock)
                    .WithData("ProductCode", productCode);
            }

            await _salesOrderManager.AddLineAsync(order, line.ProductId, line.ProductName, line.ProductCode, line.Quantity, line.UnitPriceAmount, line.Currency);
        }

        await _salesOrderRepository.InsertAsync(order, autoSave: true);
        await _backgroundJobManager.EnqueueAsync(
            new AutoCancelDraftSalesOrderArgs { SalesOrderId = order.Id },
            delay: TimeSpan.FromHours(24));

        var loadedOrder = await _salesOrderRepository.FindWithLinesAsync(order.Id) ?? order;
        return ObjectMapper.Map<SalesOrder, SalesOrderDto>(loadedOrder);
    }

    [Authorize(OrderPermissions.SalesOrders.Confirm)]
    public async Task<SalesOrderDto> ConfirmAsync(Guid id)
    {
        var order = await _salesOrderRepository.FindWithLinesAsync(id)
            ?? throw new EntityNotFoundException(typeof(SalesOrder), id);

        foreach (var line in order.OrderLines)
        {
            var stock = await _inventoryIntegrationService.CheckStockAsync(line.ProductId, line.Quantity);
            if (!stock.IsAvailable)
            {
                var productCode = string.IsNullOrWhiteSpace(stock.ProductCode) ? line.ProductCode : stock.ProductCode;
                throw new BusinessException(OrderDomainErrorCodes.InsufficientStock)
                    .WithData("ProductCode", productCode);
            }

            var reserved = await _inventoryIntegrationService.ReserveStockAsync(line.ProductId, line.Quantity, $"SO:{order.Id}:{line.Id}");
            if (!reserved)
            {
                throw new BusinessException(OrderDomainErrorCodes.ReserveStockFailed)
                    .WithData("ProductCode", line.ProductCode);
            }
        }

        await _salesOrderManager.ConfirmAsync(order);
        await _salesOrderRepository.UpdateAsync(order, autoSave: true);

        var total = order.GetTotal();
        await _distributedEventBus.PublishAsync(new SalesOrderConfirmedEto
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            Status = order.Status.ToString(),
            TotalAmount = total.Amount,
            Currency = total.Currency
        });

        return ObjectMapper.Map<SalesOrder, SalesOrderDto>(order);
    }

    [Authorize(OrderPermissions.SalesOrders.Cancel)]
    public async Task<SalesOrderDto> CancelAsync(Guid id)
    {
        var order = await _salesOrderRepository.FindWithLinesAsync(id)
            ?? throw new EntityNotFoundException(typeof(SalesOrder), id);

        await _salesOrderManager.CancelAsync(order);
        await _salesOrderRepository.UpdateAsync(order, autoSave: true);
        return ObjectMapper.Map<SalesOrder, SalesOrderDto>(order);
    }
}
