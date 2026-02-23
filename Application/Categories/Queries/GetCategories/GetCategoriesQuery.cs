using MediatR;

namespace Application.Categories.Queries.GetCategories;

public record GetCategoriesQuery() : IRequest<List<CategoryDto>>;
