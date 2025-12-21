using Contracts.Domain;
using Contracts.Front;
using Contracts.Internal.Learning;
using Microsoft.EntityFrameworkCore;
using Oh.Evaluation.Api.Abstractions;
using Oh.Evaluation.Api.ApiClients;
using Oh.Evaluation.Domain.Domain;
using Oh.Evaluation.Infrastructure;

namespace Oh.Evaluation.Api.Services;

public class StudentSubmissionService(
    ICourseClient courseClient,
    ILearningClient learningClient,
    ITextEvaluationService textEvaluationService,
    IQuizEvaluationService quizEvaluationService,
    ICodeEvaluationService codeEvaluationService,
    IUserContextService userContextService,
    EvaluationDbContext dbContext) : ISubmissionService
{
    public async Task<SubmissionResult> SubmitTask(StudentSubmissionRequest request)
    {
        var userId = userContextService.GetCurrentUserId();
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        var courseQuestion = await courseClient.GetById(request.QuestionId);
        var submission = new RunnerSubmission
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            CourseId = request.CourseId,
            PageId = request.PageId,
            QuestionId = request.QuestionId,
            
            SubmissionType = courseQuestion.Type,
            
        };
        var evaluationResult = courseQuestion.Type switch
        {
            SubmissionType.SingleChoice or SubmissionType.MultipleChoice
                => await quizEvaluationService.Evaluate(request),

            SubmissionType.TextInput
                => await textEvaluationService.Evaluate(request, courseQuestion),

            SubmissionType.Code
                => await codeEvaluationService.Evaluate(request, courseQuestion),

            _ => throw new NotSupportedException()
        };
        if (evaluationResult.Feedback is not null)
        {
            submission.AiReview = new RunnerAiReview
            {
                Id =  Guid.NewGuid(),
                Score = evaluationResult.Score,
                Suggestions = evaluationResult.Feedback.Suggestions,
                Summary = evaluationResult.Feedback.Summary,
                Status = AiReviewStatus.Done
            };
        }
        submission.Verdict = evaluationResult.Status;
        
        await dbContext.RunnerSubmissions.AddAsync(submission);
        await dbContext.SaveChangesAsync();
        
        await NotifyLearning(submission);
        return evaluationResult;
    }
    
    public Task<SubmissionResult?> GetResult(Guid submissionId)
    {
        return dbContext.Set<RunnerSubmission>()
            .Include(rs => rs.TestCaseResults)
            .Include(rs => rs.AiReview)
            .Where(rs => rs.Id == submissionId)
            .Select(rs => new SubmissionResult
            {
                Status = rs.Verdict,
                Score = rs.AiReview != null ? rs.AiReview!.Score : null,
                Feedback = rs.AiReview != null
                    ? new SubmissionFeedback
                    {
                        Suggestions = rs.AiReview!.Suggestions
                    }
                    : null,
                TestCaseResults = rs.TestCaseResults != null
                    ? rs.TestCaseResults.Select(tcr => new RunnerTestCaseResultDto()
                    {
                        Id = tcr.Id,
                        TestCaseId = tcr.Id,
                        Status = tcr.Status,
                        TimeMs = tcr.TimeMs,
                        MemoryKb = tcr.MemoryKb,
                        StderrSnippet = tcr.StderrSnippet,
                        DiffSnippet = tcr.DiffSnippet
                    })
                    : null
            }).SingleOrDefaultAsync();
    }

    private async Task NotifyLearning(RunnerSubmission submission)
    {
        await learningClient.InsertEvaluationResult(submission.CourseId!.Value, new QuestionStateUpsertRequest
        {
            Id = submission.QuestionId,
            UserId = submission.UserId,
            Status = submission.Verdict,
            Score = submission.AiReview?.Score,
            Feedback = submission.AiReview?.Summary
        });
    }
}