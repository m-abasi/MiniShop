using MediatR;

namespace Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(int Id, string Name) : IRequest<Unit>;