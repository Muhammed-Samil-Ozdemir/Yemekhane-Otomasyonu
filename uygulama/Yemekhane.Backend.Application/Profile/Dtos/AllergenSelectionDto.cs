namespace Yemekhane.Backend.Application.Profile.Dtos;

public sealed record AllergenSelectionDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public bool IsSelected { get; init; }
}
