using Mapster;
using MediatR;
using Yemekhane.Backend.Application.Cafeterias.Dtos;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Domain.Cafeterias;

namespace Yemekhane.Backend.Application.Cafeterias;

public sealed record GetAllCafeteriasQuery() : IRequest<Result<List<CafeteriaDto>>>;

internal sealed class GetAllCafeteriasQueryHandler(
    ICafeteriaRepository cafeteriaRepository) : IRequestHandler<GetAllCafeteriasQuery, Result<List<CafeteriaDto>>>
{
    public async Task<Result<List<CafeteriaDto>>> Handle(GetAllCafeteriasQuery request, CancellationToken cancellationToken)
    {
        var cafeterias = await cafeteriaRepository.GetAllAsync(cancellationToken);

        return Result<List<CafeteriaDto>>.Success(cafeterias.Adapt<List<CafeteriaDto>>());
    }
}
