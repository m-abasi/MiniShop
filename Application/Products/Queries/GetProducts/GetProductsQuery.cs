using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using MiniShop.Application.Common.Interfaces;
namespace Application.Products.Queries.GetProducts
{
    public record GetProductsQuery() : IRequest<List<ProductDto>>;
}
