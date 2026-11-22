using Yemekhane.Backend.Application.Meals.Dtos;

namespace Yemekhane.Backend.Application.Menus.Dtos;

public sealed record WeeklyMenuDto
{
    public int CafeteriaId { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public DateOnly PreviousStartDate { get; init; }
    public DateOnly NextStartDate { get; init; }
    public List<MealDto> Meals { get; init; } = [];
    public List<WeeklyMenuDayDto> Days { get; init; } = [];
}

public sealed record WeeklyMenuDayDto
{
    public DateOnly Date { get; init; }
    public bool IsToday { get; init; }
    public List<WeeklyMenuSlotDto> Slots { get; init; } = [];
}

public sealed record WeeklyMenuSlotDto
{
    public int MealId { get; init; }
    public string MealName { get; init; } = default!;
    public MenuDto? Menu { get; init; }
}
