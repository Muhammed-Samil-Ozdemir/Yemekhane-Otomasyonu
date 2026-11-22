using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Yemekhane.Backend.Application.Auth;
using Yemekhane.Backend.Application.Common;
using Yemekhane.Backend.WebAPI.Authentication;
using Yemekhane.Backend.WebAPI.ExceptionHandlers;

namespace Yemekhane.Backend.WebAPI;

public static class WebApiRegistrar
{
    public static IServiceCollection AddWebApi(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        services
            .AddAuthentication(SessionAuthenticationHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, SessionAuthenticationHandler>(SessionAuthenticationHandler.SchemeName, null);

        services.AddAuthorization();

        services.AddExceptionHandler<AppExceptionHandler>();
        services.AddProblemDetails();

        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = _ =>
                    new BadRequestObjectResult(Result.BadRequest("Gönderilen bilgiler eksik ya da hatalı."));
            });

        services.AddOpenApi();
        services.AddHealthChecks();

        return services;
    }
}
