using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace HQSOFT.Inventory.EntityFrameworkCore;

[ConnectionStringName(InventoryDbProperties.ConnectionStringName)]
public interface IInventoryDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
