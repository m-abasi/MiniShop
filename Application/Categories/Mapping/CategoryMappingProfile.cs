using Application.Categories.Queries.GetCategories;
using AutoMapper;
using Domain.Entities;

namespace Application.Categories.Mapping;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>();
    }
}
