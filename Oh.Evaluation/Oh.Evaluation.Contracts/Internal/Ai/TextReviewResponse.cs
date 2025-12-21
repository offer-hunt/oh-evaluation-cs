namespace Contracts.Internal.Ai;

public record TextReviewResponse()
{
    public required int Score { get; init; }
    public required string Summary { get; init; }
    public required string Suggestions { get; init; }
}