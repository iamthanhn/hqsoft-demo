using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace HQSOFT.Order.SaleOrders;

public interface ISalesOrderRepository : IRepository<SalesOrder, Guid>
{
    Task<string> GetNextOrderNumberAsync();
    Task<bool> ExistsOrderNumberAsync(string orderNumber);
    Task<SalesOrder?> FindWithLinesAsync(Guid id);
}
