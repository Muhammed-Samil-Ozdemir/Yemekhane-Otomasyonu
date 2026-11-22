namespace Yemekhane.Backend.Application.Evaluations.Dtos;

public sealed record EvaluationDto
{
    public int MenuId { get; init; }
    public DateOnly Date { get; init; }
    public string MealName { get; init; } = default!;
    public string CafeteriaName { get; init; } = default!;
    public string? Foods { get; init; }
    public int? Score { get; init; }
    public string? Comment { get; init; }
    public DateTime? EvaluatedAt { get; init; }
    public bool CanEvaluate { get; init; }
}
