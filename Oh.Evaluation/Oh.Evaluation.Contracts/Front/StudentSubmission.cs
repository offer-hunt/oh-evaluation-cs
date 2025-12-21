namespace Contracts.Front;

public class StudentSubmission
{
    public string? TextSubmission { get; init; }
    public IEnumerable<Guid>? QuizSubmission { get; init; }
    public string? CodeSubmission { get; init; }
}