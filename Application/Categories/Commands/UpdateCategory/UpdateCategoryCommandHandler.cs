using MediatR;
using MiniShop.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
            throw new Exception($"Category {request.Id} not found.");

        category.Name = request.Name;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}