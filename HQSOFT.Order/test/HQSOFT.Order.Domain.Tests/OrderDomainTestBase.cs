using Volo.Abp.Modularity;

namespace HQSOFT.Order;

/* Inherit from this class for your domain layer tests. */
public abstract class OrderDomainTestBase<TStartupModule> : OrderTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
