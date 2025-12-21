namespace Contracts.Internal.Course;

public record TestCaseDto
{
    public required string InputData { get; init; }
    public required string ExpectedOutput { get; init; }
}
