using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace HQSOFT.Inventory.InventoryItems;

public class InventoryItem : FullAuditedAggregateRoot<Guid>
{
    public Guid ProductId { get; private set; }
    public string ProductCode { get; private set; } = null!;
    public string ProductName { get; private set; } = null!;
    public int Quantity { get; private set; }
    public int ReservedQuantity { get; private set; }

    public int AvailableQuantity => Quantity - ReservedQuantity;

    protected InventoryItem()
    {
    }

    public InventoryItem(Guid id, Guid productId, string productCode, string productName, int quantity, int reservedQuantity = 0)
        : base(id)
    {
        Check.NotNull(productCode, nameof(productCode));
        Check.NotNull(productName, nameof(productName));

        if (quantity < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(quantity));
        if (reservedQuantity < 0)
            throw new ArgumentException("ReservedQuantity cannot be negative", nameof(reservedQuantity));
        if (reservedQuantity > quantity)
            throw new ArgumentException("ReservedQuantity cannot exceed Quantity", nameof(reservedQuantity));

        ProductId = productId;
        ProductCode = productCode;
        ProductName = productName;
        Quantity = quantity;
        ReservedQuantity = reservedQuantity;
    }

    public bool CanReserve(int quantity)
    {
        return quantity > 0 && AvailableQuantity >= quantity;
    }

    public void Reserve(int quantity, string reservationId)
    {
        if (!CanReserve(quantity))
        {
            throw new BusinessException(InventoryErrorCodes.InsufficientStock);
        }

        ReservedQuantity += quantity;
    }
}
