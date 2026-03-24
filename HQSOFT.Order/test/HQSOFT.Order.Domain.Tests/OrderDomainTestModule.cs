using Volo.Abp.Modularity;

namespace HQSOFT.Order;

[DependsOn(
    typeof(OrderDomainModule),
    typeof(OrderTestBaseModule)
)]
public class OrderDomainTestModule : AbpModule
{

}
