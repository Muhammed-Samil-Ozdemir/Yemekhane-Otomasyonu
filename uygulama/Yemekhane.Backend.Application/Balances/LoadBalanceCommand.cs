using MediatR;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Balances.Dtos;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Domain.Balances;

namespace Yemekhane.Backend.Application.Balances;

public sealed record LoadBalanceCommand(decimal Amount) : IRequest<Result<BalanceLoadReceiptDto>>;

internal sealed class LoadBalanceCommandHandler(
    IBalanceRepository balanceRepository,
    IUserContext userContext) : IRequestHandler<LoadBalanceCommand, Result<BalanceLoadReceiptDto>>
{
    public async Task<Result<BalanceLoadReceiptDto>> Handle(LoadBalanceCommand request, CancellationToken cancellationToken)
    {
        var receipt = await balanceRepository.LoadAsync(userContext.UserId, request.Amount, cancellationToken);

        return Result<BalanceLoadReceiptDto>.Created(
            new BalanceLoadReceiptDto(receipt.Balance, "Bakiye yüklendi."));
    }
}
