using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Domain.Menus;

namespace Yemekhane.Backend.Application.Menus;

internal sealed class MenuReader(
    IMenuRepository menuRepository)
{
    public Task<Menu?> GetAsync(int cafeteriaId, DateOnly date, int mealId, CancellationToken cancellationToken) =>
        menuRepository.GetAsync(cafeteriaId, date.ToDateTime(), mealId, cancellationToken);
}
