using HQSOFT.Order.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace HQSOFT.Order.Permissions;

public class OrderPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var orderGroup = context.AddGroup(OrderPermissions.GroupName);

        var salesOrders = orderGroup.AddPermission(OrderPermissions.SalesOrders.Default, L("Permission:SalesOrders"));
        salesOrders.AddChild(OrderPermissions.SalesOrders.Create, L("Permission:SalesOrders.Create"));
        salesOrders.AddChild(OrderPermissions.SalesOrders.Confirm, L("Permission:SalesOrders.Confirm"));
        salesOrders.AddChild(OrderPermissions.SalesOrders.Cancel, L("Permission:SalesOrders.Cancel"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<OrderResource>(name);
    }
}
