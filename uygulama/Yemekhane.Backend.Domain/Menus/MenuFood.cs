namespace Yemekhane.Backend.Domain.Menus;

public sealed class MenuFood
{
    public int FoodId { get; set; }
    public string Name { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
    public int CategoryOrder { get; set; }
    public int Calories { get; set; }
    public bool IsVegetarian { get; set; }
    public List<string> Allergens { get; set; } = [];
}
