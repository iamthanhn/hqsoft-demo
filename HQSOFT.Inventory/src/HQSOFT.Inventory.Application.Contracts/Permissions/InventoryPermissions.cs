using Volo.Abp.Reflection;

namespace HQSOFT.Inventory.Permissions;

public class InventoryPermissions
{
    public const string GroupName = "Inventory";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(InventoryPermissions));
    }
}
