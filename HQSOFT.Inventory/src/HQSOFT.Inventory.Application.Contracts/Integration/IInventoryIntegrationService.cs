using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HQSOFT.Inventory.Integration;

public interface IInventoryIntegrationService : IApplicationService
{
    Task<StockCheckResult> CheckStockAsync(Guid productId, int requestedQuantity);
    Task<bool> ReserveStockAsync(Guid productId, int quantity, string reservationId);
}
