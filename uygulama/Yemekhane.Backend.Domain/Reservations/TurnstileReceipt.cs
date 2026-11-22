namespace Yemekhane.Backend.Domain.Reservations;

public sealed record TurnstileReceipt(int ReservationId, int MenuId, string MealName);
