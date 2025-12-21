using Contracts.Front;

namespace Oh.Evaluation.Api.Abstractions;

public interface IQuizEvaluationService
{
    public Task<SubmissionResult> Evaluate(StudentSubmissionRequest studentSubmissionRequest);
}