namespace Contracts.Internal.Course;

public record QuizOption
{
    public Guid Id { get; init; }
    public Guid QuestionId { get; init; }
    public required string Label { get; init; } 
    public required bool Correct { get; init; }
    public int SortOrder { get; init; }
}