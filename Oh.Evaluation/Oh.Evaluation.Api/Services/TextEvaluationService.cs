using System.ComponentModel.DataAnnotations;
using Contracts.Domain;
using Contracts.Front;
using Contracts.Internal.Ai;
using Contracts.Internal.Course;
using Oh.Evaluation.Api.Abstractions;
using Oh.Evaluation.Api.ApiClients;

namespace Oh.Evaluation.Api.Services;

    public class TextEvaluationService(IAiClient aiClient) : ITextEvaluationService
{
    public async Task<SubmissionResult> Evaluate(StudentSubmissionRequest studentSubmissionRequest, CourseQuestion question)
    {
        var studentAnswer = studentSubmissionRequest.SubmissionData.TextSubmission;
        if (string.IsNullOrWhiteSpace(studentAnswer))
        {
            throw new ValidationException();
        }
        
        if (studentAnswer == question.CorrectAnswer)
        {
            return new SubmissionResult
            {
                Status = EvaluationStatus.Accepted
            };
        }
        
        var status = EvaluationStatus.Rejected;

        if (question.UseAiCheck)
        {
            var aiResponse = await aiClient.GetTextReview(new TextReviewRequest
            {
                Answer = studentAnswer,
                Question = question.Text,
                Rubric = question.CorrectAnswer!
            });

            return new SubmissionResult
            {
                Status = status,
                Score = aiResponse.Score,
                Feedback = new SubmissionFeedback
                { 
                    Summary = aiResponse.Summary,
                    Suggestions = aiResponse.Suggestions
                }
            };
        }

        return new SubmissionResult
        {
            Status = status
        };
    }
}