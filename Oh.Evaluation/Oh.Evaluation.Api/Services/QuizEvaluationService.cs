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

        if (quizAnswers.Count != studentAnswers.Count)
        {
            return new SubmissionResult
            {
                Status = status
            };
        }

        for (var i = 0; i < quizAnswers.Count; i++)
        {
            if (quizAnswers[i].Correct && (studentAnswers[i] != quizAnswers[i].Id))
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