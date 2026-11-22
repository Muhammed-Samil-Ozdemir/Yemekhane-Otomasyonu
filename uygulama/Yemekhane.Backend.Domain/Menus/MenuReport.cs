namespace Yemekhane.Backend.Domain.Menus;

public sealed class MenuReport
{
    public int MenuId { get; set; }
    public int CafeteriaId { get; set; }
    public string CafeteriaName { get; set; } = default!;
    public DateTime Date { get; set; }
    public int MealId { get; set; }
    public string MealName { get; set; } = default!;
    public int Capacity { get; set; }
    public int ReservationCount { get; set; }
    public int FreeSeats { get; set; }
    public decimal OccupancyRate { get; set; }
    public int EvaluationCount { get; set; }
    public decimal? AverageScore { get; set; }
}
