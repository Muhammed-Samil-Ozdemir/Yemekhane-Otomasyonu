namespace Yemekhane.Backend.Domain.Evaluations;

public interface IEvaluationRepository
{
    Task<List<EvaluableMenu>> GetByUserAsync(int userId, CancellationToken cancellationToken);

    Task CreateAsync(int userId, int menuId, int score, string? comment, CancellationToken cancellationToken);
}
