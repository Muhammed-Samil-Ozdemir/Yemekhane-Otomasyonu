namespace Yemekhane.Backend.Application.Auth;

public interface ISessionService
{
    Task<string> CreateAsync(SessionUser user, CancellationToken cancellationToken = default);
    Task<SessionUser?> GetAsync(string token, CancellationToken cancellationToken = default);
    Task RemoveAsync(string token, CancellationToken cancellationToken = default);
}
