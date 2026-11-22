using Microsoft.AspNetCore.Diagnostics;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.Domain.Abstractions;

namespace Yemekhane.Backend.WebAPI.ExceptionHandlers;

public sealed class AppExceptionHandler(
    ILogger<AppExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        Result result;

        if (exception is BusinessRuleException businessRuleException)
        {
            logger.LogInformation("İş kuralı reddetti: {Message}", businessRuleException.Message);
            result = Result.BadRequest(businessRuleException.Message);
        }
        else
        {
            logger.LogError(exception, "İstek işlenirken beklenmeyen bir hata oluştu.");
            result = Result.ServerError("Beklenmeyen bir hata oluştu. Lütfen biraz sonra tekrar deneyin.");
        }

        httpContext.Response.StatusCode = (int)result.Status;
        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);

        return true;
    }
}
