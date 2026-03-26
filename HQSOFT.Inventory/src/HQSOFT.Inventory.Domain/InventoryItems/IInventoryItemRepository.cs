using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HQSOFT.Inventory.InventoryItems;

public interface IInventoryItemRepository : IRepository<InventoryItem, Guid>
{
    Task<InventoryItem?> FindByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}
