namespace Yemekhane.Backend.Domain.Balances;

public interface IBalanceRepository
{
    Task<BalanceLoadReceipt> LoadAsync(int userId, decimal amount, CancellationToken cancellationToken);

    Task<decimal> GetBalanceAsync(int userId, CancellationToken cancellationToken);

    Task<List<BalanceMovement>> GetMovementsAsync(int userId, int count, CancellationToken cancellationToken);
}
