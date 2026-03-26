using System;
using System.Threading.Tasks;
using HQSOFT.Inventory.InventoryItems;

namespace HQSOFT.Inventory.Integration;

public class InventoryIntegrationService : InventoryAppService, IInventoryIntegrationService
{
    private readonly IInventoryItemRepository _repository;

    public InventoryIntegrationService(IInventoryItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<StockCheckResult> CheckStockAsync(Guid productId, int requestedQuantity)
    {
        var item = await _repository.FindByProductIdAsync(productId);

        if (item is null)
        {
            return new StockCheckResult
            {
                IsAvailable = false,
                AvailableQuantity = 0,
                ProductCode = string.Empty,
                ProductName = string.Empty
            };
        }

        return new StockCheckResult
        {
            IsAvailable = item.AvailableQuantity >= requestedQuantity,
            AvailableQuantity = item.AvailableQuantity,
            ProductCode = item.ProductCode,
            ProductName = item.ProductName
        };
    }

    public async Task<bool> ReserveStockAsync(Guid productId, int quantity, string reservationId)
    {
        var item = await _repository.FindByProductIdAsync(productId);

        if (item == null)
        {
            return false;
        }

        if (!item.CanReserve(quantity))
        {
            return false;
        }

        item.Reserve(quantity, reservationId);
        await _repository.UpdateAsync(item, autoSave: true);

        return true;
    }
}
