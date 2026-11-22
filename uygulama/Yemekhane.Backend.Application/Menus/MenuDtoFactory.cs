using Yemekhane.Backend.Application.Menus.Dtos;
using Yemekhane.Backend.Domain.Menus;
using Yemekhane.Backend.Domain.Reservations;

namespace Yemekhane.Backend.Application.Menus;

internal static class MenuDtoFactory
{
    public static MenuDto Create(
        Menu menu,
        string role,
        DateTime now,
        IReadOnlySet<string> userAllergens,
        MenuOccupancy? occupancy,
        Reservation? reservation)
    {
        var myReservation = reservation is { Status: not ReservationStatuses.Cancelled } ? reservation : null;
        var isOpen = menu.IsReservationOpen(now);
        var hasFreeSeat = occupancy is null || occupancy.FreeSeats > 0;

        var foods = menu.Foods
            .OrderBy(food => food.CategoryOrder)
            .ThenBy(food => food.Name)
            .Select(food => CreateFood(food, userAllergens))
            .ToList();

        return new MenuDto
        {
            Id = menu.Id,
            CafeteriaId = menu.CafeteriaId,
            CafeteriaName = menu.CafeteriaName,
            Date = DateOnly.FromDateTime(menu.Date),
            MealId = menu.MealId,
            MealName = menu.MealName,
            ReservationDeadline = menu.ReservationDeadline,
            Price = menu.GetPrice(role),
            HalfPrice = menu.GetHalfPrice(role),
            TotalCalories = menu.GetTotalCalories(),
            IsVegetarian = menu.IsVegetarian(),
            HasAllergenWarning = foods.Any(food => food.HasAllergenWarning),
            AllergenWarnings = foods.SelectMany(food => food.AllergenWarnings).Distinct().Order().ToList(),
            Capacity = occupancy?.Capacity,
            FreeSeats = occupancy?.FreeSeats,
            IsReservable = isOpen && hasFreeSeat && myReservation is null,
            ReservationNote = GetReservationNote(myReservation, isOpen, hasFreeSeat),
            MyReservation = myReservation is null
                ? null
                : new MyReservationDto
                {
                    Id = myReservation.Id,
                    Status = myReservation.Status,
                    StatusText = ReservationStatuses.GetText(myReservation.Status),
                    Portion = myReservation.Portion,
                    PortionText = Portions.GetText(myReservation.Portion)
                },
            Foods = foods
        };
    }

    private static MenuFoodDto CreateFood(MenuFood food, IReadOnlySet<string> userAllergens)
    {
        var warnings = food.Allergens
            .Where(userAllergens.Contains)
            .ToList();

        return new MenuFoodDto
        {
            Id = food.FoodId,
            Name = food.Name,
            CategoryName = food.CategoryName,
            Calories = food.Calories,
            IsVegetarian = food.IsVegetarian,
            Allergens = food.Allergens,
            AllergenWarnings = warnings,
            HasAllergenWarning = warnings.Count > 0
        };
    }

    private static string? GetReservationNote(Reservation? myReservation, bool isOpen, bool hasFreeSeat)
    {
        if (myReservation is not null)
            return $"Rezervasyonunuz var ({Portions.GetText(myReservation.Portion).ToLowerInvariant()})";

        if (!isOpen)
            return "Rezervasyon süresi doldu";

        return hasFreeSeat ? null : "Boş yer kalmadı";
    }
}
