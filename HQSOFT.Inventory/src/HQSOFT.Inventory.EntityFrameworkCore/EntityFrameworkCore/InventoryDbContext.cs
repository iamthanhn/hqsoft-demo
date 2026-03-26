using HQSOFT.Inventory.InventoryItems;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace HQSOFT.Inventory.EntityFrameworkCore;

[ConnectionStringName(InventoryDbProperties.ConnectionStringName)]
public class InventoryDbContext : AbpDbContext<InventoryDbContext>, IInventoryDbContext
{
    public DbSet<InventoryItem> InventoryItems { get; set; } = null!;

    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureInventory();
    }
}
