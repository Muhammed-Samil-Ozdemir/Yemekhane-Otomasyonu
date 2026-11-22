namespace Yemekhane.Backend.Application.Menus.Dtos;

public sealed record MenuFoodDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string CategoryName { get; init; } = default!;
    public int Calories { get; init; }
    public bool IsVegetarian { get; init; }
    public List<string> Allergens { get; init; } = [];
    public List<string> AllergenWarnings { get; init; } = [];
    public bool HasAllergenWarning { get; init; }
}
