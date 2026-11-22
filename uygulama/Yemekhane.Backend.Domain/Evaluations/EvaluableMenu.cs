namespace Yemekhane.Backend.Domain.Evaluations;

public sealed class EvaluableMenu
{
    public int MenuId { get; set; }
    public DateTime Date { get; set; }
    public string MealName { get; set; } = default!;
    public string CafeteriaName { get; set; } = default!;
    public string? Foods { get; set; }
    public int? Score { get; set; }
    public string? Comment { get; set; }
    public DateTime? EvaluatedAt { get; set; }

    public bool IsEvaluated => Score is not null;
}
