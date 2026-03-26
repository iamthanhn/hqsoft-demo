using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HQSOFT.Order.SaleOrders;

public interface ISalesOrderAppService : IApplicationService
{
    Task<PagedResultDto<SalesOrderDto>> GetListAsync(GetSalesOrdersInput input);
    Task<SalesOrderDto> GetAsync(Guid id);
    Task<SalesOrderDto> CreateAsync(CreateSalesOrderDto input);
    Task<SalesOrderDto> ConfirmAsync(Guid id);
    Task<SalesOrderDto> CancelAsync(Guid id);
}
