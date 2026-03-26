using Volo.Abp.Modularity;

namespace HQSOFT.Inventory;

[DependsOn(
    typeof(InventoryDomainModule),
    typeof(InventoryTestBaseModule)
)]
public class InventoryDomainTestModule : AbpModule
{

}
