using MediatR;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Domain.Allergens;

namespace Yemekhane.Backend.Application.Profile;

public sealed record UpdateMyAllergensCommand(List<int> AllergenIds) : IRequest<Result<string>>;

internal sealed class UpdateMyAllergensCommandHandler(
    IAllergenRepository allergenRepository,
    IUserContext userContext) : IRequestHandler<UpdateMyAllergensCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateMyAllergensCommand request, CancellationToken cancellationToken)
    {
        var allergenIds = request.AllergenIds.Distinct().ToList();

        await allergenRepository.SetUserAllergensAsync(userContext.UserId, allergenIds, cancellationToken);

        return Result<string>.Success(allergenIds.Count == 0
            ? "Alerjen listeniz temizlendi."
            : $"{allergenIds.Count} alerjen kaydedildi. Menülerde bu alerjenleri içeren yemekler uyarıyla gösterilecek.");
    }
}
