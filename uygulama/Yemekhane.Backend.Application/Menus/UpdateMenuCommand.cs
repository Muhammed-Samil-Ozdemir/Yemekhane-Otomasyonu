using Mapster;
using MediatR;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Domain.Menus;

namespace Yemekhane.Backend.Application.Menus;

public sealed record UpdateMenuCommand(
    int Id,
    int CafeteriaId,
    DateOnly Date,
    int MealId,
    decimal StudentPrice,
    decimal StaffPrice,
    List<int> FoodIds) : IRequest<Result<string>>;

internal sealed class UpdateMenuCommandHandler(
    IMenuRepository menuRepository) : IRequestHandler<UpdateMenuCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        var errors = MenuCommandValidator.Validate(request.Date, request.StudentPrice, request.StaffPrice, request.FoodIds);
        if (errors.Length > 0)
            return Result<string>.BadRequest(errors);

        await menuRepository.UpdateAsync(request.Adapt<MenuDefinition>(), cancellationToken);

        return Result<string>.Success("Menü güncellendi.");
    }
}
