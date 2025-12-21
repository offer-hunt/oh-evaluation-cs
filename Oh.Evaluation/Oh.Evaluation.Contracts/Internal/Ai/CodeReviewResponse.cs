namespace Contracts.Internal.Ai;

public record CodeReviewResponse
{
    public required int Score { get; init; }
    public required string Summary { get; init; }
    public required IEnumerable<string> Suggestions { get; init; }
}