using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Yemekhane.Backend.Application.Reservations;
using Yemekhane.Backend.WebAPI.Abstractions;

namespace Yemekhane.Backend.WebAPI.Controllers;

[Route("api/reservations")]
[ApiController]
[Authorize]
public sealed class ReservationController(
    IMediator mediator) : CustomBaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(new GetMyReservationsQuery(), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(CreateReservationCommand request, CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(request, cancellationToken));

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken) =>
        CreateActionResult(await mediator.Send(new CancelReservationCommand(id), cancellationToken));
}
