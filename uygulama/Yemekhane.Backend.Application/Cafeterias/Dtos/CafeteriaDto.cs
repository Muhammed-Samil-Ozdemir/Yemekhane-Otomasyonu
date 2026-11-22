namespace Yemekhane.Backend.Application.Cafeterias.Dtos;

public sealed record CafeteriaDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Location { get; init; }
    public int Capacity { get; init; }
}
