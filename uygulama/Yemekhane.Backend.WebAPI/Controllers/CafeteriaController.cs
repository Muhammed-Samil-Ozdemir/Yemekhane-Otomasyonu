using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yemekhane.Backend.Application.Cafeterias;
using Yemekhane.Backend.WebAPI.Abstractions;

namespace Yemekhane.Backend.WebAPI.Controllers;

[Route("api/cafeterias")]
[ApiController]
[Authorize]
public sealed class CafeteriaController(
    IMediator mediator) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(new GetAllCafeteriasQuery(), cancellationToken));
}
