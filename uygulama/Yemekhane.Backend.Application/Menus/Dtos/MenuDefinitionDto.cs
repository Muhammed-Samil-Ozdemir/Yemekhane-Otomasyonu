namespace Yemekhane.Backend.Application.Menus.Dtos;

public sealed record MenuDefinitionDto
{
    public int Id { get; init; }
    public int CafeteriaId { get; init; }
    public DateOnly Date { get; init; }
    public int MealId { get; init; }
    public decimal StudentPrice { get; init; }
    public decimal StaffPrice { get; init; }
    public List<int> FoodIds { get; init; } = [];
}
