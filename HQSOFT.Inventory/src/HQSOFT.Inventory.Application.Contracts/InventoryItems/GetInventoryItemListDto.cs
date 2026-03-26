using System;
using Volo.Abp.Application.Dtos;

namespace HQSOFT.Inventory.InventoryItems;

public class GetInventoryItemListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? ProductId { get; set; }
}
