using AutoMapper;
using HQSOFT.Order.SaleOrders;
using System.Linq;

namespace HQSOFT.Order;

public class OrderApplicationAutoMapperProfile : Profile
{
    public OrderApplicationAutoMapperProfile()
    {
        CreateMap<SalesOrderLine, SalesOrderLineDto>()
            .ForMember(dest => dest.UnitPriceAmount, opt => opt.MapFrom(src => src.UnitPrice.Amount))
            .ForMember(dest => dest.LineTotalAmount, opt => opt.MapFrom(src => src.LineTotal.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.LineTotal.Currency));

        CreateMap<SalesOrder, SalesOrderDto>()
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.GetTotal().Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.GetTotal().Currency))
            .ForMember(dest => dest.OrderLines, opt => opt.MapFrom(src => src.OrderLines.ToList()));
    }
}
