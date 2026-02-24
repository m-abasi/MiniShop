using MediatR;
using MiniShop.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, string?>
{
    private readonly IApplicationDbContext _context;

    public DeleteCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string?> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
            return "دسته‌بندی پیدا نشد.";

        var hasProducts = await _context.Products
            .AnyAsync(p => p.CategoryId == request.Id, cancellationToken);

        if (hasProducts)
            return "این دسته‌بندی دارای محصول است و قابل حذف نمی‌باشد.";

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return null;
    }
}