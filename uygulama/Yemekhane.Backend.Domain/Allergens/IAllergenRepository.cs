namespace Yemekhane.Backend.Domain.Allergens;

public interface IAllergenRepository
{
    Task<List<Allergen>> GetAllAsync(CancellationToken cancellationToken);

    Task<List<Allergen>> GetUserAllergensAsync(int userId, CancellationToken cancellationToken);

    Task SetUserAllergensAsync(int userId, IReadOnlyCollection<int> allergenIds, CancellationToken cancellationToken);
}
