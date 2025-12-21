using Contracts.Front;
using Contracts.Internal.Course;
using Oh.Evaluation.Api.Abstractions;

namespace Oh.Evaluation.Api.Services;

public class CodeEvaluationService : ICodeEvaluationService
{
    public Task<SubmissionResult> Evaluate(StudentSubmissionRequest studentSubmissionRequest, CourseQuestion question)
    {
        throw new NotImplementedException();
    }
}