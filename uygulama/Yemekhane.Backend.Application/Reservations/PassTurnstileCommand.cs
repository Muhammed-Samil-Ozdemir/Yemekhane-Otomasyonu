using MediatR;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Domain.Reservations;
using Yemekhane.Backend.Domain.Users;

namespace Yemekhane.Backend.Application.Reservations;

public sealed record PassTurnstileCommand(
    string Email,
    int CafeteriaId,
    DateTime? Time) : IRequest<Result<string>>;

internal sealed class PassTurnstileCommandHandler(
    IUserRepository userRepository,
    IReservationRepository reservationRepository) : IRequestHandler<PassTurnstileCommand, Result<string>>
{
    public async Task<Result<string>> Handle(PassTurnstileCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);
        if (user is null)
            return Result<string>.NotFound("Bu e-posta ile kayıtlı kullanıcı yok.");

        var receipt = await reservationRepository.PassTurnstileAsync(user.Id, request.CafeteriaId, request.Time, cancellationToken);

        return Result<string>.Success($"{user.FullName}, {receipt.MealName} öğünü için turnikeden geçti.");
    }
}
