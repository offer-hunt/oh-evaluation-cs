using System.Text.Json;
using Contracts.Domain;

namespace Contracts.Front;

public record SubmissionResult
{
    public EvaluationStatus Status { get; set; }
    public decimal? Score { get; set; }
    public SubmissionFeedback? Feedback { get; set; }
    public IEnumerable<RunnerTestCaseResultDto>? TestCaseResults { get; set; }
}