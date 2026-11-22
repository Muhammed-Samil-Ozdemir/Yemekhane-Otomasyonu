namespace Yemekhane.Backend.Application.Menus.Dtos;

public sealed record MenuDto
{
    public int Id { get; init; }
    public int CafeteriaId { get; init; }
    public string CafeteriaName { get; init; } = default!;
    public DateOnly Date { get; init; }
    public int MealId { get; init; }
    public string MealName { get; init; } = default!;
    public DateTime ReservationDeadline { get; init; }
    public decimal Price { get; init; }
    public decimal? HalfPrice { get; init; }
    public int TotalCalories { get; init; }
    public bool IsVegetarian { get; init; }
    public bool HasAllergenWarning { get; init; }
    public List<string> AllergenWarnings { get; init; } = [];
    public int? Capacity { get; init; }
    public int? FreeSeats { get; init; }
    public bool IsReservable { get; init; }
    public string? ReservationNote { get; init; }
    public MyReservationDto? MyReservation { get; init; }
    public List<MenuFoodDto> Foods { get; init; } = [];
}
