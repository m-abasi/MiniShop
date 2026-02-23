using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Products.Commands.CreateProducts
{
    public record CreateProductCommand(
        string Name,
        string Description,
        decimal Price,
        int Stock,
        int CategoryId
    ) : IRequest<int>;

}
