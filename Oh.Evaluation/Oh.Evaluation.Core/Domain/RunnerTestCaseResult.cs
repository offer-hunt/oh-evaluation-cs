using Contracts.Domain;

namespace Oh.Evaluation.Domain.Domain;

public class RunnerTestCaseResult
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public Guid? TestCaseId { get; set; }
    public TestCaseStatus Status { get; set; }
    public int? TimeMs { get; set; }
    public int? MemoryKb { get; set; }
        
    public string? StderrSnippet { get; set; }
        
    public string? DiffSnippet { get; set; }
        
    public Guid RunnerSubmissionId { get; set; }

    public RunnerSubmission Submission { get; set; } = null!;
}