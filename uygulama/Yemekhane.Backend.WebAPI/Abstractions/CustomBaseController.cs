using Microsoft.AspNetCore.Mvc;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Application.Common.Enums;

namespace Yemekhane.Backend.WebAPI.Abstractions;

[Route("api/[controller]")]
[ApiController]
public class CustomBaseController : ControllerBase
{
    [NonAction]
    public IActionResult CreateActionResult<T>(Result<T> result)
    {
        return result.Status switch
        {
            ResultStatus.Ok =>
                Ok(result),

            ResultStatus.Created =>
                StatusCode(StatusCodes.Status201Created, result),

            ResultStatus.BadRequest =>
                BadRequest(result),

            ResultStatus.NotFound =>
                NotFound(result),

            ResultStatus.Unauthorized =>
                Unauthorized(result),

            ResultStatus.Forbidden =>
                StatusCode(StatusCodes.Status403Forbidden, result),

            ResultStatus.Conflict =>
                Conflict(result),

            _ =>
                StatusCode(StatusCodes.Status500InternalServerError, result)
        };
    }
}
