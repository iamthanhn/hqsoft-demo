using System;
using System.Linq;
using System.Threading.Tasks;
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

        var dtos = items.Select(item => new InventoryItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductCode = item.ProductCode,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            ReservedQuantity = item.ReservedQuantity,
            AvailableQuantity = item.AvailableQuantity
        }).ToList();

        return new PagedResultDto<InventoryItemDto>(totalCount, dtos);
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
        // ABP's FullAuditedAggregateRoot doesn't allow direct property updates
        // For update, we need to create methods in the entity or use a different approach
        // For now, this is a placeholder - you may need to add Update methods to InventoryItem entity
    }

    protected override InventoryItemDto MapToGetOutputDto(InventoryItem entity)
    {
        return new InventoryItemDto
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            ProductCode = entity.ProductCode,
            ProductName = entity.ProductName,
            Quantity = entity.Quantity,
            ReservedQuantity = entity.ReservedQuantity,
            AvailableQuantity = entity.AvailableQuantity
        };
    }

    protected override InventoryItemDto MapToGetListOutputDto(InventoryItem entity)
    {
        return MapToGetOutputDto(entity);
    }
}
