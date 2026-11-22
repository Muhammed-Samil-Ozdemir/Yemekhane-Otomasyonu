using Mapster;
using MediatR;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Application.Menus.Dtos;
using Yemekhane.Backend.Domain.Menus;

namespace Yemekhane.Backend.Application.Menus;

public sealed record GetMenuDefinitionQuery(int Id) : IRequest<Result<MenuDefinitionDto>>;

internal sealed class GetMenuDefinitionQueryHandler(
    IMenuRepository menuRepository) : IRequestHandler<GetMenuDefinitionQuery, Result<MenuDefinitionDto>>
{
    public async Task<Result<MenuDefinitionDto>> Handle(GetMenuDefinitionQuery request, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetDefinitionAsync(request.Id, cancellationToken);

        return menu is null
            ? Result<MenuDefinitionDto>.NotFound("Menü bulunamadı.")
            : Result<MenuDefinitionDto>.Success(menu.Adapt<MenuDefinitionDto>());
    }
}
