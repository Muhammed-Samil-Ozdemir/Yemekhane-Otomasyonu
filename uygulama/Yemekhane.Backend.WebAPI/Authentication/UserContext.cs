using System.Security.Claims;
using Yemekhane.Backend.Application.Auth;

namespace Yemekhane.Backend.WebAPI.Authentication;

public sealed class UserContext(
    IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public int UserId =>
        int.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : 0;

    public string Role =>
        User?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    public string? SessionToken =>
        User?.FindFirstValue(SessionAuthenticationHandler.TokenClaimType);
}
