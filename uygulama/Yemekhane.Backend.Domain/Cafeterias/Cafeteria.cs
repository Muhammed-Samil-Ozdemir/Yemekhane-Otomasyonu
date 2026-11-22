using Yemekhane.Backend.Domain.Abstractions;

namespace Yemekhane.Backend.Domain.Cafeterias;

public sealed class Cafeteria : Entity
{
    public string Name { get; set; } = default!;
    public string? Location { get; set; }
    public int Capacity { get; set; }
}
