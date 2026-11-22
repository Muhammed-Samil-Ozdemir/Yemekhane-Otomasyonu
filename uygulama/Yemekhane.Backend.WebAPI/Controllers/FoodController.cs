using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yemekhane.Backend.Application.Foods;
using Yemekhane.Backend.Domain.Users;
using Yemekhane.Backend.WebAPI.Abstractions;

namespace Yemekhane.Backend.WebAPI.Controllers;

[Route("api/admin/foods")]
[ApiController]
[Authorize(Roles = UserRoles.Admin)]
public sealed class FoodController(
    IMediator mediator) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(new GetAllFoodsQuery(), cancellationToken));
}
