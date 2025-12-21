using Contracts.Domain;

namespace Contracts.Internal.Learning;

public record QuestionStateUpsertRequest
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public EvaluationStatus Status { get; init; }
    public decimal? Score { get; init; }
    public string? Feedback { get; init; }
}