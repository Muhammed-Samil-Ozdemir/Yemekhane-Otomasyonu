using Mapster;
using MediatR;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Application.Meals.Dtos;
using Yemekhane.Backend.Application.Menus.Dtos;
using Yemekhane.Backend.Domain.Meals;
using Yemekhane.Backend.Domain.Menus;
using Yemekhane.Backend.Domain.Reservations;

namespace Yemekhane.Backend.Application.Menus;

public sealed record GetWeeklyMenuQuery(
    int CafeteriaId,
    DateOnly? StartDate) : IRequest<Result<WeeklyMenuDto>>;

internal sealed class GetWeeklyMenuQueryHandler(
    MenuReader menuReader,
    IMealRepository mealRepository,
    IMenuRepository menuRepository,
    IReservationRepository reservationRepository,
    IUserContext userContext) : IRequestHandler<GetWeeklyMenuQuery, Result<WeeklyMenuDto>>
{
    private const int DayCount = 7;

    public async Task<Result<WeeklyMenuDto>> Handle(GetWeeklyMenuQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);
        var startDate = request.StartDate ?? today;
        var endDate = startDate.AddDays(DayCount - 1);

        var meals = await mealRepository.GetAllAsync(cancellationToken);

        var occupancies = (await menuRepository.GetOccupanciesAsync(
                request.CafeteriaId, startDate.ToDateTime(), endDate.ToDateTime(), cancellationToken))
            .ToDictionary(occupancy => occupancy.MenuId);

        var reservations = (await reservationRepository.GetByUserBetweenAsync(
                userContext.UserId, request.CafeteriaId, startDate.ToDateTime(), endDate.ToDateTime(), cancellationToken))
            .ToDictionary(reservation => reservation.MenuId);

        var days = new List<WeeklyMenuDayDto>();

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var slots = new List<WeeklyMenuSlotDto>();

            foreach (var meal in meals)
            {
                var menu = await menuReader.GetAsync(request.CafeteriaId, date, meal.Id, cancellationToken);

                slots.Add(new WeeklyMenuSlotDto
                {
                    MealId = meal.Id,
                    MealName = meal.Name,
                    Menu = menu is null
                        ? null
                        : MenuDtoFactory.Create(
                            menu,
                            userContext.Role,
                            now,
                            occupancies.GetValueOrDefault(menu.Id),
                            reservations.GetValueOrDefault(menu.Id))
                });
            }

            days.Add(new WeeklyMenuDayDto
            {
                Date = date,
                IsToday = date == today,
                Slots = slots
            });
        }

        return Result<WeeklyMenuDto>.Success(new WeeklyMenuDto
        {
            CafeteriaId = request.CafeteriaId,
            StartDate = startDate,
            EndDate = endDate,
            PreviousStartDate = startDate.AddDays(-DayCount),
            NextStartDate = startDate.AddDays(DayCount),
            Meals = meals.Adapt<List<MealDto>>(),
            Days = days
        });
    }
}
