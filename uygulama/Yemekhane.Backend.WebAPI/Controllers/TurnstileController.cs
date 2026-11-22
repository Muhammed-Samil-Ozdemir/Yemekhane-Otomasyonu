using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yemekhane.Backend.Application.Reservations;
using Yemekhane.Backend.Domain.Users;
using Yemekhane.Backend.WebAPI.Abstractions;

namespace Yemekhane.Backend.WebAPI.Controllers;

[Route("api/admin/turnstile")]
[ApiController]
[Authorize(Roles = UserRoles.Admin)]
public sealed class TurnstileController(
    IMediator mediator) : CustomBaseController
{
    [HttpPost]
    public async Task<IActionResult> Pass(PassTurnstileCommand request, CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(request, cancellationToken));
}
