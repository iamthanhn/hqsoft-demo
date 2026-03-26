using System;

namespace HQSOFT.Inventory.InventoryItems;

public class CreateUpdateInventoryItemDto
{
    public Guid ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
}
