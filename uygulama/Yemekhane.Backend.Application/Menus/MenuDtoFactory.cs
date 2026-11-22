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
        MenuOccupancy? occupancy,
        Reservation? reservation)
    {
        var myReservation = reservation is { Status: not ReservationStatuses.Cancelled } ? reservation : null;
        var isOpen = menu.IsReservationOpen(now);
        var hasFreeSeat = occupancy is null || occupancy.FreeSeats > 0;

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
            TotalCalories = menu.GetTotalCalories(),
            IsVegetarian = menu.IsVegetarian(),
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
                    StatusText = ReservationStatuses.GetText(myReservation.Status)
                },
            Foods = menu.Foods
                .OrderBy(food => food.CategoryOrder)
                .ThenBy(food => food.Name)
                .Select(food => new MenuFoodDto
                {
                    Id = food.FoodId,
                    Name = food.Name,
                    CategoryName = food.CategoryName,
                    Calories = food.Calories,
                    IsVegetarian = food.IsVegetarian,
                    Allergens = food.Allergens
                })
                .ToList()
        };
    }

    private static string? GetReservationNote(Reservation? myReservation, bool isOpen, bool hasFreeSeat)
    {
        if (myReservation is not null)
            return $"Rezervasyonunuz var ({ReservationStatuses.GetText(myReservation.Status).ToLowerInvariant()})";

        if (!isOpen)
            return "Rezervasyon süresi doldu";

        return hasFreeSeat ? null : "Boş yer kalmadı";
    }
}
