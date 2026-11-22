using Scalar.AspNetCore;
using Yemekhane.Backend.Application;
using Yemekhane.Backend.Infrastructure;
using Yemekhane.Backend.WebAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddWebApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/api/openapi/{documentName}.json");
    app.MapScalarApiReference("/api/scalar", options =>
        options.WithOpenApiRoutePattern("/api/openapi/{documentName}.json"));
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/api/health");

app.Run();
