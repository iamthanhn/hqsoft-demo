using Volo.Abp.Modularity;

namespace HQSOFT.Inventory;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class InventoryApplicationTestBase<TStartupModule> : InventoryTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
