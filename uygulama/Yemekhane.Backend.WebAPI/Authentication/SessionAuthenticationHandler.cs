using System.Globalization;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Common;

namespace Yemekhane.Backend.WebAPI.Authentication;

public sealed class SessionAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISessionService sessionService) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "Oturum";
    public const string CookieName = "oturum";
    public const string TokenClaimType = "oturum_token";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Cookies.TryGetValue(CookieName, out var token) || string.IsNullOrWhiteSpace(token))
            return AuthenticateResult.NoResult();

        var session = await sessionService.GetAsync(token, Context.RequestAborted);
        if (session is null)
            return AuthenticateResult.Fail("Oturum bulunamadı ya da süresi doldu.");

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, session.UserId.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.Name, session.FullName),
            new(ClaimTypes.Email, session.Email),
            new(ClaimTypes.Role, session.Role),
            new(TokenClaimType, token)
        ];

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));

        return AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName));
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        await Response.WriteAsJsonAsync(
            Result.Unauthorized("Oturumunuz kapalı ya da süresi doldu. Lütfen giriş yapın."));
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        await Response.WriteAsJsonAsync(
            Result.Forbidden("Bu işlem için yetkiniz yok."));
    }
}
