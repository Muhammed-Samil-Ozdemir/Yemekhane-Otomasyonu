namespace Yemekhane.Backend.Domain.Meals;

public interface IMealRepository
{
    Task<List<Meal>> GetAllAsync(CancellationToken cancellationToken);
}
