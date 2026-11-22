namespace Yemekhane.Backend.Application.Balances.Dtos;

public sealed record BalanceLoadReceiptDto(
    decimal Balance,
    string Message);
