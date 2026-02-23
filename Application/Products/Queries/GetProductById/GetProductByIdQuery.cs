using MediatR;
using Application.Products.Queries.GetProducts;

namespace Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;
