using Mapster;
using MediatR;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Balances.Dtos;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Domain.Balances;

namespace Yemekhane.Backend.Application.Balances;

public sealed record GetBalanceQuery() : IRequest<Result<BalanceDto>>;

internal sealed class GetBalanceQueryHandler(
    IBalanceRepository balanceRepository,
    IUserContext userContext) : IRequestHandler<GetBalanceQuery, Result<BalanceDto>>
{
    private const int MovementCount = 50;

    public async Task<Result<BalanceDto>> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var balance = await balanceRepository.GetBalanceAsync(userContext.UserId, cancellationToken);
        var movements = await balanceRepository.GetMovementsAsync(userContext.UserId, MovementCount, cancellationToken);

        return Result<BalanceDto>.Success(
            new BalanceDto(balance, movements.Adapt<List<BalanceMovementDto>>()));
    }
}
