using MediatR;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Application.Reservations.Dtos;
using Yemekhane.Backend.Domain.Reservations;

namespace Yemekhane.Backend.Application.Reservations;

public sealed record CreateReservationCommand(
    int MenuId,
    string? Portion) : IRequest<Result<ReservationReceiptDto>>;

internal sealed class CreateReservationCommandHandler(
    IReservationRepository reservationRepository,
    IUserContext userContext) : IRequestHandler<CreateReservationCommand, Result<ReservationReceiptDto>>
{
    public async Task<Result<ReservationReceiptDto>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        var portion = string.IsNullOrWhiteSpace(request.Portion) ? Portions.Full : request.Portion;

        var receipt = await reservationRepository.CreateAsync(userContext.UserId, request.MenuId, portion, cancellationToken);

        return Result<ReservationReceiptDto>.Created(new ReservationReceiptDto(
            receipt.ReservationId,
            receipt.Amount,
            receipt.Balance,
            $"Rezervasyonunuz alındı ({Portions.GetText(portion).ToLowerInvariant()})."));
    }
}
