using HQSOFT.Order.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace HQSOFT.Order.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(OrderEntityFrameworkCoreModule),
    typeof(OrderApplicationContractsModule)
)]
public class OrderDbMigratorModule : AbpModule
{
}
