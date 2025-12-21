using Contracts.Domain;
using Contracts.Front;
using Oh.Evaluation.Api.Abstractions;
using Oh.Evaluation.Api.ApiClients;

namespace Oh.Evaluation.Api.Services;

public class QuizEvaluationService(ICourseClient courseClient) : IQuizEvaluationService
{
    public async Task<SubmissionResult> Evaluate(StudentSubmissionRequest studentSubmissionRequest)
    {
        var quizAnswers = (await courseClient.GetQuizOptions(studentSubmissionRequest.QuestionId)).ToList();
        var studentAnswers = studentSubmissionRequest.SubmissionData.QuizSubmission?.ToList() ?? [];
        
        var status = EvaluationStatus.Accepted;

        foreach (var quizAnswer in quizAnswers)
        {
            if (quizAnswer.Correct && !studentAnswers.Contains(quizAnswer.Id))
            {
                status = EvaluationStatus.Rejected;
            }
        }
        
        foreach (var studentAnswer in studentAnswers)
        {
            if (quizAnswers.Select(q => q.Id).Contains(studentAnswer) && 
                quizAnswers.Any(q => q.Id == studentAnswer && !q.Correct))
            {
                status = EvaluationStatus.Rejected;
            }
        }
        
        return new SubmissionResult
        {
            Status = status
        };
    }
}