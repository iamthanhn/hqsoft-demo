using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HQSOFT.Inventory.InventoryItems;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HQSOFT.Inventory.InventoryItems;

public class InventoryItemAppService : CrudAppService<
    InventoryItem,
    InventoryItemDto,
    Guid,
    GetInventoryItemListDto,
    CreateUpdateInventoryItemDto,
    CreateUpdateInventoryItemDto>, IInventoryItemAppService
{
    private readonly IInventoryItemRepository _repository;

    public InventoryItemAppService(IInventoryItemRepository repository)
        : base(repository)
    {
        _repository = repository;
    }

    protected override async Task<IQueryable<InventoryItem>> CreateFilteredQueryAsync(GetInventoryItemListDto input)
    {
        var query = await base.CreateFilteredQueryAsync(input);

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            query = query.Where(x =>
                x.ProductCode.Contains(input.Filter) ||
                x.ProductName.Contains(input.Filter));
        }

        if (input.ProductId.HasValue)
        {
            query = query.Where(x => x.ProductId == input.ProductId.Value);
        }

        return query;
    }

    public async Task<PagedResultDto<InventoryItemDto>> GetProductListAsync(GetInventoryItemListDto input)
    {
        var query = await CreateFilteredQueryAsync(input);

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = ApplySorting(query, input);
        query = ApplyPaging(query, input);

        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<InventoryItemDto>(
            totalCount,
            ObjectMapper.Map<List<InventoryItem>, List<InventoryItemDto>>(items)
        );
    }

    protected override InventoryItem MapToEntity(CreateUpdateInventoryItemDto createInput)
    {
        return new InventoryItem(
            GuidGenerator.Create(),
            createInput.ProductId,
            createInput.ProductCode,
            createInput.ProductName,
            createInput.Quantity,
            createInput.ReservedQuantity
        );
    }

    protected override void MapToEntity(CreateUpdateInventoryItemDto updateInput, InventoryItem entity)
    {
        // Entity uses private setters, so we need to use domain methods
        // For now, this is a placeholder - you may need to add Update methods to InventoryItem entity
    }
}
