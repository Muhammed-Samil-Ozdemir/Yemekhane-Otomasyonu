namespace Yemekhane.Backend.Infrastructure.Menus;

internal sealed class MenuDetailRow
{
    public int MenuId { get; set; }
    public int CafeteriaId { get; set; }
    public string CafeteriaName { get; set; } = default!;
    public DateTime Date { get; set; }
    public int MealId { get; set; }
    public string MealName { get; set; } = default!;
    public DateTime ReservationDeadline { get; set; }
    public decimal StudentPrice { get; set; }
    public decimal StaffPrice { get; set; }
    public int FoodId { get; set; }
    public string FoodName { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
    public int CategoryOrder { get; set; }
    public int Calories { get; set; }
    public bool IsVegetarian { get; set; }
    public string? Allergens { get; set; }
}
