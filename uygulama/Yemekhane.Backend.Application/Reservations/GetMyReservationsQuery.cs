using Mapster;
using MediatR;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Application.Reservations.Dtos;
using Yemekhane.Backend.Domain.Reservations;

namespace Yemekhane.Backend.Application.Reservations;

public sealed record GetMyReservationsQuery() : IRequest<Result<List<ReservationDto>>>;

internal sealed class GetMyReservationsQueryHandler(
    IReservationRepository reservationRepository,
    IUserContext userContext) : IRequestHandler<GetMyReservationsQuery, Result<List<ReservationDto>>>
{
    public async Task<Result<List<ReservationDto>>> Handle(GetMyReservationsQuery request, CancellationToken cancellationToken)
    {
        var reservations = await reservationRepository.GetByUserAsync(userContext.UserId, cancellationToken);

        return Result<List<ReservationDto>>.Success(reservations.Adapt<List<ReservationDto>>());
    }
}
