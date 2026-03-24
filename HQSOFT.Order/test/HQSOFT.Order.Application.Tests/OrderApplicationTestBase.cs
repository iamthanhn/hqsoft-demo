using Volo.Abp.Modularity;

namespace HQSOFT.Order;

public abstract class OrderApplicationTestBase<TStartupModule> : OrderTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
