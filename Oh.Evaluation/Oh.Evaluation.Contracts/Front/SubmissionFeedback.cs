namespace Contracts.Front;

public record SubmissionFeedback
{
    public string? Summary { get; set; }
    public string? Suggestions { get; set; }
}