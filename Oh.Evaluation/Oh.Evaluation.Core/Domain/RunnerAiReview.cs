namespace Oh.Evaluation.Domain.Domain;

public class RunnerAiReview
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public AiReviewStatus Status { get; set; }
    
    public string? Model { get; set; }
    public decimal? Score { get; set; }
    public string? Summary { get; set; }
    public string? Suggestions { get; set; }
        
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    
    public Guid RunnerSubmissionId { get; set; }

    public RunnerSubmission Submission { get; set; } = null!;
}