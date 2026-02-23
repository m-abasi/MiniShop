using Application.Orders.Queries.GetOrders;
using AutoMapper;
using Domain.Entities;

namespace Application.Orders.Mapping;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Items,     opt => opt.MapFrom(src => src.OrderItems));
    }
}
