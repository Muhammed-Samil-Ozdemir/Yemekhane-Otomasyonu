using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yemekhane.Backend.Application.Profile;
using Yemekhane.Backend.WebAPI.Abstractions;

namespace Yemekhane.Backend.WebAPI.Controllers;

[Route("api/profile")]
[ApiController]
[Authorize]
public sealed class ProfileController(
    IMediator mediator) : CustomBaseController
{
    [HttpGet("allergens")]
    public async Task<IActionResult> GetAllergens(CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(new GetMyAllergensQuery(), cancellationToken));

    [HttpPut("allergens")]
    public async Task<IActionResult> UpdateAllergens(UpdateMyAllergensCommand request, CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(request, cancellationToken));
}
