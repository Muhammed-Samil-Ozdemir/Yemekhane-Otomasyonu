FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Yemekhane.Backend.Domain/Yemekhane.Backend.Domain.csproj                 Yemekhane.Backend.Domain/
COPY Yemekhane.Backend.Application/Yemekhane.Backend.Application.csproj       Yemekhane.Backend.Application/
COPY Yemekhane.Backend.Infrastructure/Yemekhane.Backend.Infrastructure.csproj Yemekhane.Backend.Infrastructure/
COPY Yemekhane.Backend.WebAPI/Yemekhane.Backend.WebAPI.csproj                 Yemekhane.Backend.WebAPI/
RUN dotnet restore Yemekhane.Backend.WebAPI/Yemekhane.Backend.WebAPI.csproj

COPY . .
RUN dotnet publish Yemekhane.Backend.WebAPI/Yemekhane.Backend.WebAPI.csproj \
    -c Release \
    -o /app \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app .

USER $APP_UID

ENV ASPNETCORE_HTTP_PORTS=8000
EXPOSE 8000

ENTRYPOINT ["dotnet", "Yemekhane.Backend.WebAPI.dll"]
