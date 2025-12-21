using Contracts.Domain;

namespace Contracts.Internal.Course;

public record CourseQuestion
{
    public Guid Id { get; init; }
    public Guid PageId { get; init; }
    public required SubmissionType Type { get; init; }
    public required string Text { get; init; }
    public string? CorrectAnswer { get; init; }
    public bool UseAiCheck { get; init; }
    public int Points { get; init; }
    public int SortOrder { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}