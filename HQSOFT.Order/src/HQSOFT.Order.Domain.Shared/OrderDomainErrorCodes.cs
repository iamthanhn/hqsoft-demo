namespace HQSOFT.Order;

public static class OrderDomainErrorCodes
{
    public const string InsufficientStock = "Order:010001";
    public const string ReserveStockFailed = "Order:010002";
    public const string OrderNotFound = "Order:010003";
    public const string InvalidOrderStatus = "Order:010004";
    public const string NoProductsInOrder = "Order:010005";
    public const string DuplicateOrderNumber = "Order:010006";
}
