using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using HQSOFT.Inventory.Integration;

namespace HQSOFT.Order.SaleOrders;

public class FakeInventoryIntegrationService : IInventoryIntegrationService
{
    private readonly ConcurrentDictionary<Guid, StockState> _stocks = [];

    public ConcurrentBag<(Guid ProductId, int Quantity, string ReservationId)> ReserveCalls { get; } = [];

    public void SetStock(Guid productId, bool isAvailable, int availableQuantity, string productCode, string productName, bool reserveResult)
    {
        _stocks[productId] = new StockState(isAvailable, availableQuantity, productCode, productName, reserveResult);
    }

    public Task<StockCheckResult> CheckStockAsync(Guid productId, int requestedQuantity)
    {
        if (!_stocks.TryGetValue(productId, out var stock))
        {
            return Task.FromResult(new StockCheckResult
            {
                IsAvailable = false,
                AvailableQuantity = 0,
                ProductCode = string.Empty,
                ProductName = string.Empty
            });
        }

        return Task.FromResult(new StockCheckResult
        {
            IsAvailable = stock.IsAvailable && stock.AvailableQuantity >= requestedQuantity,
            AvailableQuantity = stock.AvailableQuantity,
            ProductCode = stock.ProductCode,
            ProductName = stock.ProductName
        });
    }

    public Task<bool> ReserveStockAsync(Guid productId, int quantity, string reservationId)
    {
        ReserveCalls.Add((productId, quantity, reservationId));
        return Task.FromResult(_stocks.TryGetValue(productId, out var stock) && stock.ReserveResult);
    }

    private sealed record StockState(bool IsAvailable, int AvailableQuantity, string ProductCode, string ProductName, bool ReserveResult);
}
