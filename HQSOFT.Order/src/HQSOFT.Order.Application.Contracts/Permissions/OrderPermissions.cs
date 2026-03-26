namespace HQSOFT.Order.Permissions;

public static class OrderPermissions
{
    public const string GroupName = "Order";

    public static class SalesOrders
    {
        public const string Default = GroupName + ".SalesOrders";
        public const string Create = Default + ".Create";
        public const string Confirm = Default + ".Confirm";
        public const string Cancel = Default + ".Cancel";
    }
}
