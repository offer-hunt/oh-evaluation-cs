namespace Contracts.Internal.Ai;

public record TextReviewRequest
{
    public required string Answer { get; init; }
    public required string Question { get; init; }
    public required string Rubric { get; init; }
}