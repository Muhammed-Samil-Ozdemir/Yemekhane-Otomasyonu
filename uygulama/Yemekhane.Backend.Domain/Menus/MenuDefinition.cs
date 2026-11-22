using Yemekhane.Backend.Domain.Abstractions;

namespace Yemekhane.Backend.Domain.Menus;

public sealed class MenuDefinition : Entity
{
    public int CafeteriaId { get; set; }
    public DateTime Date { get; set; }
    public int MealId { get; set; }
    public decimal StudentPrice { get; set; }
    public decimal StaffPrice { get; set; }
    public List<int> FoodIds { get; set; } = [];
}
