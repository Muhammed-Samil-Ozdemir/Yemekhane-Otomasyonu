namespace Yemekhane.Backend.Domain.Menus;

public interface IMenuRepository
{
    Task<Menu?> GetAsync(int cafeteriaId, DateTime date, int mealId, CancellationToken cancellationToken);

    Task<List<MenuOccupancy>> GetOccupanciesAsync(int cafeteriaId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);

    Task<List<MenuReport>> GetReportAsync(int? cafeteriaId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);

    Task<MenuDefinition?> GetDefinitionAsync(int id, CancellationToken cancellationToken);

    Task<int> CreateAsync(MenuDefinition menu, CancellationToken cancellationToken);

    Task UpdateAsync(MenuDefinition menu, CancellationToken cancellationToken);
}
