using Contracts.Front;
using Contracts.Internal.Course;

namespace Oh.Evaluation.Api.Abstractions;

public interface ITextEvaluationService
{
    public Task<SubmissionResult> Evaluate(StudentSubmissionRequest studentSubmissionRequest, CourseQuestion question);
}