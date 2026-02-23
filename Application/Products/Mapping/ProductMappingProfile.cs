using Application.Products.Commands.CreateProducts;
using Application.Products.Queries.GetProducts;
using Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;

namespace MiniShop.Application.Products.Mapping;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

        // اگر لازم شد می‌تونی برای CreateProductCommand -> Product هم mapping بذاری
        CreateMap<CreateProductCommand, Product>();
    }
}
