namespace Contracts.Front;

public class StudentSubmissionRequest
{
    public Guid CourseId { get; init; }
    public Guid PageId { get; init; }
    public Guid QuestionId { get; init; }
    public required StudentSubmission SubmissionData { get; init; }
}