using HQSOFT.Inventory.EntityFrameworkCore;
using HQSOFT.Order.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace HQSOFT.Order.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(OrderEntityFrameworkCoreModule),
    typeof(OrderApplicationContractsModule)
)]

[DependsOn(typeof(InventoryEntityFrameworkCoreModule))]
public class OrderDbMigratorModule : AbpModule
{
}
