using HQSOFT.Inventory.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace HQSOFT.Inventory;

public abstract class InventoryController : AbpControllerBase
{
    protected InventoryController()
    {
        LocalizationResource = typeof(InventoryResource);
    }
}
