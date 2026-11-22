namespace Yemekhane.Backend.Domain.Cafeterias;

public interface ICafeteriaRepository
{
    Task<List<Cafeteria>> GetAllAsync(CancellationToken cancellationToken);
}
