using HQSOFT.Inventory.EntityFrameworkCore.Repositories;
using HQSOFT.Inventory.InventoryItems;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace HQSOFT.Inventory.EntityFrameworkCore;

[DependsOn(
    typeof(InventoryDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class InventoryEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<InventoryDbContext>(options =>
        {
            options.AddDefaultRepositories<IInventoryDbContext>(includeAllEntities: true);
            options.AddRepository<InventoryItem, EfCoreInventoryItemRepository>();
        });
    }
}
