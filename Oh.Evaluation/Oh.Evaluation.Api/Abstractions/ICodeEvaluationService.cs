using Contracts.Front;
using Contracts.Internal.Course;

namespace Oh.Evaluation.Api.Abstractions;

public interface ICodeEvaluationService
{
    public Task<SubmissionResult> Evaluate(StudentSubmissionRequest studentSubmissionRequest, CourseQuestion question);
}