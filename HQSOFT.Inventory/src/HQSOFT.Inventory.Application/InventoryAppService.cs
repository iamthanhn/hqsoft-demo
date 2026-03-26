using HQSOFT.Inventory.Localization;
using Volo.Abp.Application.Services;

namespace HQSOFT.Inventory;

public abstract class InventoryAppService : ApplicationService
{
    protected InventoryAppService()
    {
        LocalizationResource = typeof(InventoryResource);
        ObjectMapperContext = typeof(InventoryApplicationModule);
    }
}
