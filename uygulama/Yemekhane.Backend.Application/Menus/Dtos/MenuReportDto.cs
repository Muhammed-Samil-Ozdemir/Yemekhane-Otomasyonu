namespace Yemekhane.Backend.Application.Menus.Dtos;

public sealed record MenuReportDto
{
    public int MenuId { get; init; }
    public int CafeteriaId { get; init; }
    public string CafeteriaName { get; init; } = default!;
    public DateOnly Date { get; init; }
    public int MealId { get; init; }
    public string MealName { get; init; } = default!;
    public int Capacity { get; init; }
    public int ReservationCount { get; init; }
    public int FreeSeats { get; init; }
    public decimal OccupancyRate { get; init; }
    public int EvaluationCount { get; init; }
    public decimal? AverageScore { get; init; }
}
