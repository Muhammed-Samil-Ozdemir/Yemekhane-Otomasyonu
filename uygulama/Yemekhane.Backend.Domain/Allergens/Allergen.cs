using Yemekhane.Backend.Domain.Abstractions;

namespace Yemekhane.Backend.Domain.Allergens;

public sealed class Allergen : Entity
{
    public string Name { get; set; } = default!;
}
