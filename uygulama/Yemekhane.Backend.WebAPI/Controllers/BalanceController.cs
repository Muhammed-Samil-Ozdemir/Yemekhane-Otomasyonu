using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yemekhane.Backend.Application.Balances;
using Yemekhane.Backend.WebAPI.Abstractions;

namespace Yemekhane.Backend.WebAPI.Controllers;

[Route("api/balance")]
[ApiController]
[Authorize]
public sealed class BalanceController(
    IMediator mediator) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(new GetBalanceQuery(), cancellationToken));

    [HttpPost("load")]
    public async Task<IActionResult> Load(LoadBalanceCommand request, CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(request, cancellationToken));
}
