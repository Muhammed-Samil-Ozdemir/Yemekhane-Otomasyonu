namespace Yemekhane.Backend.Application.Balances.Dtos;

public sealed record BalanceMovementDto
{
    public int Id { get; init; }
    public string Type { get; init; } = default!;
    public string TypeText { get; init; } = default!;
    public decimal Amount { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
}
