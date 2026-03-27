using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HQSOFT.Inventory.Integration;

public interface IInventoryIntegrationService : IApplicationService
{
    Task<StockCheckResult> CheckStockAsync(Guid productId, int requestedQuantity);
    Task<bool> ReserveStockAsync(Guid productId, int quantity, string reservationId);
    Task<bool> ReleaseStockAsync(Guid productId, int quantity, string reservationId);
}

public class StockCheckResult
{
    public bool IsAvailable { get; set; }
    public int AvailableQuantity { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
}
