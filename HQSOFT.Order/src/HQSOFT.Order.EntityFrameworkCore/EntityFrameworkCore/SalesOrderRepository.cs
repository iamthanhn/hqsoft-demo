using System;
using System.Linq;
using System.Threading.Tasks;
using HQSOFT.Order.SaleOrders;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace HQSOFT.Order.EntityFrameworkCore;

public class SalesOrderRepository : EfCoreRepository<OrderDbContext, SalesOrder, Guid>,
    ISalesOrderRepository
{
    public SalesOrderRepository(IDbContextProvider<OrderDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<string> GetNextOrderNumberAsync()
    {
        var dbSet = await GetDbSetAsync();
        var today = DateTime.UtcNow.Date;
        var prefix = $"SO-{today:yyyyMMdd}-";

        var lastOrder = await dbSet
            .Where(x => x.OrderNumber.StartsWith(prefix))
            .OrderByDescending(x => x.OrderNumber)
            .FirstOrDefaultAsync();

        if (lastOrder == null)
            return $"{prefix}001";

        var lastCounterStr = lastOrder.OrderNumber.Substring(prefix.Length);
        if (int.TryParse(lastCounterStr, out var lastCounter))
            return $"{prefix}{(lastCounter + 1):D3}";

        return $"{prefix}001";
    }

    public async Task<bool> ExistsOrderNumberAsync(string orderNumber)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x => x.OrderNumber == orderNumber);
    }
}
