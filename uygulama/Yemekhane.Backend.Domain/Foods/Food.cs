using Yemekhane.Backend.Domain.Abstractions;

namespace Yemekhane.Backend.Domain.Foods;

public sealed class Food : Entity
{
    public string Name { get; set; } = default!;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public int CategoryOrder { get; set; }
    public int Calories { get; set; }
    public bool IsVegetarian { get; set; }
    public string? Allergens { get; set; }
}
