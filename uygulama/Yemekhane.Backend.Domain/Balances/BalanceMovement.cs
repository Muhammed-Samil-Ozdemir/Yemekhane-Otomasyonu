using Yemekhane.Backend.Domain.Abstractions;

namespace Yemekhane.Backend.Domain.Balances;

public sealed class BalanceMovement : Entity
{
    public string Type { get; set; } = default!;
    public decimal Amount { get; set; }
    public int? ReservationId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
