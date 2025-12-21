using Contracts.Internal.Course;

namespace Contracts.Internal.Ai;

public record CodeReviewRequest
{
    public required string Code { get; init; }
    public required string Language { get; init; }
    public required string TaskDescription { get; init; }
    public required IEnumerable<TestCaseDto> TestCases { get; init; }
}