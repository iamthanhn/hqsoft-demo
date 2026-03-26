using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HQSOFT.Inventory.InventoryItems;

public interface IInventoryItemAppService : ICrudAppService<
    InventoryItemDto,
    Guid,
    GetInventoryItemListDto,
    CreateUpdateInventoryItemDto,
    CreateUpdateInventoryItemDto>
{
    Task<PagedResultDto<InventoryItemDto>> GetProductListAsync(GetInventoryItemListDto input);
}
