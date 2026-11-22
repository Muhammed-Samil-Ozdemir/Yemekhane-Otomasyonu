namespace Yemekhane.Backend.Application.Balances.Dtos;

public sealed record BalanceDto(
    decimal Balance,
    List<BalanceMovementDto> Movements);
