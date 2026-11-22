using MediatR;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Application.Profile.Dtos;
using Yemekhane.Backend.Domain.Allergens;

namespace Yemekhane.Backend.Application.Profile;

public sealed record GetMyAllergensQuery() : IRequest<Result<List<AllergenSelectionDto>>>;

internal sealed class GetMyAllergensQueryHandler(
    IAllergenRepository allergenRepository,
    IUserContext userContext) : IRequestHandler<GetMyAllergensQuery, Result<List<AllergenSelectionDto>>>
{
    public async Task<Result<List<AllergenSelectionDto>>> Handle(GetMyAllergensQuery request, CancellationToken cancellationToken)
    {
        var allergens = await allergenRepository.GetAllAsync(cancellationToken);
        var selectedIds = (await allergenRepository.GetUserAllergensAsync(userContext.UserId, cancellationToken))
            .Select(allergen => allergen.Id)
            .ToHashSet();

        var result = allergens
            .Select(allergen => new AllergenSelectionDto
            {
                Id = allergen.Id,
                Name = allergen.Name,
                IsSelected = selectedIds.Contains(allergen.Id)
            })
            .ToList();

        return Result<List<AllergenSelectionDto>>.Success(result);
    }
}
