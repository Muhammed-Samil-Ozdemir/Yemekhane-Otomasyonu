namespace Yemekhane.Backend.Domain.Foods;

public interface IFoodRepository
{
    Task<List<Food>> GetAllAsync(CancellationToken cancellationToken);
}
