using Volo.Abp.Modularity;

namespace HQSOFT.Inventory;

[DependsOn(
    typeof(InventoryApplicationModule),
    typeof(InventoryDomainTestModule)
    )]
public class InventoryApplicationTestModule : AbpModule
{

}
