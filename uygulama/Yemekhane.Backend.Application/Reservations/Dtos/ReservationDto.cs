namespace Yemekhane.Backend.Application.Reservations.Dtos;

public sealed record ReservationDto
{
    public int Id { get; init; }
    public int MenuId { get; init; }
    public DateOnly Date { get; init; }
    public string MealName { get; init; } = default!;
    public string CafeteriaName { get; init; } = default!;
    public DateTime ReservationDeadline { get; init; }
    public string Status { get; init; } = default!;
    public string StatusText { get; init; } = default!;
    public string Portion { get; init; } = default!;
    public string PortionText { get; init; } = default!;
    public decimal Price { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool CanCancel { get; init; }
}
