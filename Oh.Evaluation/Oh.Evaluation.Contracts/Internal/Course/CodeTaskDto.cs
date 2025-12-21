namespace Contracts.Internal.Course;

public record CodeTaskDto
{
    public required string Description { get; init; }
    public required string Language { get; init; }
    public required List<TestCaseDto> TestCases { get; init; }
}
