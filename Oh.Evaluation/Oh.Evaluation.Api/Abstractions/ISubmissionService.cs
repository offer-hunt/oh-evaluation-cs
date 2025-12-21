using Contracts.Front;

namespace Oh.Evaluation.Api.Abstractions;

public interface ISubmissionService
{
    public Task<SubmissionResult?> GetResult(Guid submissionId);

    public Task<SubmissionResult> SubmitTask(StudentSubmissionRequest request);
}