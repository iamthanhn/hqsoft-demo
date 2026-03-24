using Volo.Abp.Modularity;

namespace HQSOFT.Order;

[DependsOn(
    typeof(OrderApplicationModule),
    typeof(OrderDomainTestModule)
)]
public class OrderApplicationTestModule : AbpModule
{

}
