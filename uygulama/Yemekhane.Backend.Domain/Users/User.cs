using Yemekhane.Backend.Domain.Abstractions;

namespace Yemekhane.Backend.Domain.Users;

public sealed class User : Entity
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string? StudentNumber { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}
