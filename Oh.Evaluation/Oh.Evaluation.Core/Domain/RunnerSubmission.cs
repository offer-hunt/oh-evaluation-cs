using System.Text.Json;
using Contracts;
using Contracts.Domain;

namespace Oh.Evaluation.Domain.Domain;

public class RunnerSubmission
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? LessonId { get; set; }
    public Guid PageId { get; set; }
    public Guid QuestionId { get; set; }
        
    public SubmissionType SubmissionType { get; set; }
        
    public string? Language { get; set; }
        
    public string? RuntimeImage { get; set; }
        
    public int? TimeLimitMs { get; set; }
    public int? MemoryLimitMb { get; set; }
    public string Code { get; set; }
    public DockerPipelineStatus Status { get; set; }
    public EvaluationStatus Verdict { get; set; }
    public int TestsPassed { get; set; }
    public int TestsTotal { get; set; }
    public int? TimeMs { get; set; }
    public int? MemoryKb { get; set; }
        
    public string? Result { get; set; }
    
    public DateTimeOffset SubmittedAt { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
        
    public List<RunnerTestCaseResult>? TestCaseResults { get; set; }
    public RunnerAiReview? AiReview { get; set; }
}