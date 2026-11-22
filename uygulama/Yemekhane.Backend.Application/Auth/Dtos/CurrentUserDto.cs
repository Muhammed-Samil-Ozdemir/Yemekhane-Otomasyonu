namespace Yemekhane.Backend.Application.Auth.Dtos;

public sealed record CurrentUserDto
{
    public int Id { get; init; }
    public string FullName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Role { get; init; } = default!;
    public string RoleText { get; init; } = default!;
    public string? StudentNumber { get; init; }
    public decimal Balance { get; init; }
}
