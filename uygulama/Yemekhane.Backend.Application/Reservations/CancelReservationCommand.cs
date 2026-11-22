using MediatR;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Application.Reservations.Dtos;
using Yemekhane.Backend.Domain.Reservations;

namespace Yemekhane.Backend.Application.Reservations;

public sealed record CancelReservationCommand(int ReservationId) : IRequest<Result<ReservationReceiptDto>>;

internal sealed class CancelReservationCommandHandler(
    IReservationRepository reservationRepository,
    IUserContext userContext) : IRequestHandler<CancelReservationCommand, Result<ReservationReceiptDto>>
{
    public async Task<Result<ReservationReceiptDto>> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var receipt = await reservationRepository.CancelAsync(userContext.UserId, request.ReservationId, cancellationToken);

        return Result<ReservationReceiptDto>.Success(new ReservationReceiptDto(
            receipt.ReservationId,
            receipt.Amount,
            receipt.Balance,
            "Rezervasyonunuz iptal edildi, ücret bakiyenize iade edildi."));
    }
}
