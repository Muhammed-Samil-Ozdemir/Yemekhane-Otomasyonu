using Yemekhane.Backend.Domain.Abstractions;
using Yemekhane.Backend.Domain.Users;

namespace Yemekhane.Backend.Domain.Menus;

public sealed class Menu : Entity
{
    public int CafeteriaId { get; set; }
    public string CafeteriaName { get; set; } = default!;
    public DateTime Date { get; set; }
    public int MealId { get; set; }
    public string MealName { get; set; } = default!;
    public DateTime ReservationDeadline { get; set; }
    public decimal StudentPrice { get; set; }
    public decimal StaffPrice { get; set; }
    public List<MenuFood> Foods { get; set; } = [];

    public decimal GetPrice(string role) =>
        role == UserRoles.Student ? StudentPrice : StaffPrice;

    public int GetTotalCalories() =>
        Foods.Sum(food => food.Calories);

    public bool IsVegetarian() =>
        Foods.Count > 0 && Foods.All(food => food.IsVegetarian);

    public bool IsReservationOpen(DateTime now) =>
        now < ReservationDeadline;
}
