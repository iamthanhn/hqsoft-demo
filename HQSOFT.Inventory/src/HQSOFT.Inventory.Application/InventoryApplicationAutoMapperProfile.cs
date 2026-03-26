using AutoMapper;
using HQSOFT.Inventory.InventoryItems;

namespace HQSOFT.Inventory;

public class InventoryApplicationAutoMapperProfile : Profile
{
    public InventoryApplicationAutoMapperProfile()
    {
        CreateMap<InventoryItem, InventoryItemDto>();
        CreateMap<CreateUpdateInventoryItemDto, InventoryItem>();
    }
}
