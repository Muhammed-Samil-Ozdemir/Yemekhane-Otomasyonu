using Mapster;
using MediatR;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Domain.Menus;

namespace Yemekhane.Backend.Application.Menus;

public sealed record CreateMenuCommand(
    int CafeteriaId,
    DateOnly Date,
    int MealId,
    decimal StudentPrice,
    decimal StaffPrice,
    List<int> FoodIds) : IRequest<Result<int>>;

internal sealed class CreateMenuCommandHandler(
    IMenuRepository menuRepository,
    MenuCacheInvalidator menuCacheInvalidator) : IRequestHandler<CreateMenuCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        var errors = MenuCommandValidator.Validate(request.Date, request.StudentPrice, request.StaffPrice, request.FoodIds);
        if (errors.Length > 0)
            return Result<int>.BadRequest(errors);

        var menu = request.Adapt<MenuDefinition>();
        var menuId = await menuRepository.CreateAsync(menu, cancellationToken);

        await menuCacheInvalidator.InvalidateAsync([menu], cancellationToken);

        return Result<int>.Created(menuId);
    }
}
