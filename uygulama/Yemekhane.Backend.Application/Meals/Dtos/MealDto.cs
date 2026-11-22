namespace Yemekhane.Backend.Application.Meals.Dtos;

public sealed record MealDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string StartTime { get; init; } = default!;
    public string EndTime { get; init; } = default!;
    public string ReservationDeadline { get; init; } = default!;
}
