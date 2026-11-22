using MediatR;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Application.Menus.Dtos;
using Yemekhane.Backend.Domain.Allergens;
using Yemekhane.Backend.Domain.Menus;
using Yemekhane.Backend.Domain.Reservations;

namespace Yemekhane.Backend.Application.Menus;

public sealed record GetDailyMenuQuery(
    int CafeteriaId,
    int MealId,
    DateOnly? Date) : IRequest<Result<MenuDto>>;

internal sealed class GetDailyMenuQueryHandler(
    MenuReader menuReader,
    IMenuRepository menuRepository,
    IReservationRepository reservationRepository,
    IAllergenRepository allergenRepository,
    IUserContext userContext) : IRequestHandler<GetDailyMenuQuery, Result<MenuDto>>
{
    public async Task<Result<MenuDto>> Handle(GetDailyMenuQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var date = request.Date ?? DateOnly.FromDateTime(now);

        var menu = await menuReader.GetAsync(request.CafeteriaId, date, request.MealId, cancellationToken);
        if (menu is null)
            return Result<MenuDto>.NotFound("Seçilen gün ve öğün için yayınlanmış bir menü yok.");

        var occupancies = await menuRepository.GetOccupanciesAsync(
            request.CafeteriaId, date.ToDateTime(), date.ToDateTime(), cancellationToken);

        var reservations = await reservationRepository.GetByUserBetweenAsync(
            userContext.UserId, request.CafeteriaId, date.ToDateTime(), date.ToDateTime(), cancellationToken);

        var userAllergens = await allergenRepository.GetUserAllergensAsync(userContext.UserId, cancellationToken);

        var result = MenuDtoFactory.Create(
            menu,
            userContext.Role,
            now,
            userAllergens.Select(allergen => allergen.Name).ToHashSet(),
            occupancies.FirstOrDefault(occupancy => occupancy.MenuId == menu.Id),
            reservations.FirstOrDefault(reservation => reservation.MenuId == menu.Id));

        return Result<MenuDto>.Success(result);
    }
}
