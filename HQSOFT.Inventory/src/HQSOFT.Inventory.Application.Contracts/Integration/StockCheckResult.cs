namespace HQSOFT.Inventory.Integration;

public class StockCheckResult
{
    public bool IsAvailable { get; set; }
    public int AvailableQuantity { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
}
