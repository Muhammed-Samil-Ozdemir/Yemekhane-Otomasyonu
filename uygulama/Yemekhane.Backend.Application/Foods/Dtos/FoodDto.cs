namespace Yemekhane.Backend.Application.Foods.Dtos;

public sealed record FoodDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = default!;
    public int Calories { get; init; }
    public bool IsVegetarian { get; init; }
    public string? Allergens { get; init; }
}
