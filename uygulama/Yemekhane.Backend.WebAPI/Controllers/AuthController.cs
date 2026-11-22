using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.WebAPI.Abstractions;
using Yemekhane.Backend.WebAPI.Authentication;

namespace Yemekhane.Backend.WebAPI.Controllers;

[Route("api/auth")]
[ApiController]
[Authorize]
public sealed class AuthController(
    IMediator mediator) : CustomBaseController
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);

        if (result.IsSuccess)
        {
            Response.Cookies.Append(SessionAuthenticationHandler.CookieName, result.Value.Token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = Request.IsHttps,
                IsEssential = true,
                Path = "/"
            });
        }

        return CreateActionResult(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LogoutCommand(), cancellationToken);

        Response.Cookies.Delete(SessionAuthenticationHandler.CookieName, new CookieOptions { Path = "/" });

        return CreateActionResult(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(new GetCurrentUserQuery(), cancellationToken));
}
