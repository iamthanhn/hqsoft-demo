using System;
using System.Threading;
using System.Threading.Tasks;
using HQSOFT.Inventory.InventoryItems;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace HQSOFT.Inventory.EntityFrameworkCore.Repositories;

public class EfCoreInventoryItemRepository
    : EfCoreRepository<InventoryDbContext, InventoryItem, Guid>, IInventoryItemRepository
{
    public EfCoreInventoryItemRepository(IDbContextProvider<InventoryDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<InventoryItem?> FindByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .FirstOrDefaultAsync(x => x.ProductId == productId, cancellationToken);
    }
}
