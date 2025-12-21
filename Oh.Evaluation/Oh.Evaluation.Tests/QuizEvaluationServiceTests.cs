using Contracts.Domain;
using Contracts.Front;
using Contracts.Internal.Course;
using Moq;
using Oh.Evaluation.Api.ApiClients;
using Oh.Evaluation.Api.Services;

namespace Oh.Evaluation.Tests;

public class QuizEvaluationServiceTests
{
    private readonly Mock<ICourseClient> _courseClientMock;
    private readonly QuizEvaluationService _quizEvaluationService;

    public QuizEvaluationServiceTests()
    {
        _courseClientMock = new Mock<ICourseClient>();
        _quizEvaluationService = new QuizEvaluationService(_courseClientMock.Object);
    }

    [Fact]
    public async Task Evaluate_AllCorrectAnswersSelected_ReturnsAccepted()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var correctOption1 = new QuizOption { Id = Guid.NewGuid(), Label = "ans", QuestionId = questionId, Correct = true };
        var correctOption2 = new QuizOption { Id = Guid.NewGuid(), Label = "ans", QuestionId = questionId, Correct = true };
        var incorrectOption = new QuizOption { Id = Guid.NewGuid(), Label = "ans", QuestionId = questionId, Correct = false };

        var quizOptions = new List<QuizOption> { correctOption1, correctOption2, incorrectOption };
        _courseClientMock.Setup(x => x.GetQuizOptions(questionId)).ReturnsAsync(quizOptions);

        var request = new StudentSubmissionRequest
        {
            QuestionId = questionId,
            SubmissionData = new StudentSubmission
            {
                QuizSubmission = new List<Guid> { correctOption1.Id, correctOption2.Id }
            }
        };

        // Act
        var result = await _quizEvaluationService.Evaluate(request);

        // Assert
        Assert.Equal(EvaluationStatus.Accepted, result.Status);
        _courseClientMock.Verify(x => x.GetQuizOptions(questionId), Times.Once);
    }

    [Fact]
    public async Task Evaluate_SomeCorrectAnswersNotSelected_ReturnsRejected()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var correctOption1 = new QuizOption { Id = Guid.NewGuid(), Label = "ans", QuestionId = questionId, Correct = true };
        var correctOption2 = new QuizOption { Id = Guid.NewGuid(), Label = "ans", QuestionId = questionId, Correct = true };
        var incorrectOption = new QuizOption { Id = Guid.NewGuid(), Label = "ans", QuestionId = questionId, Correct = false };

        var quizOptions = new List<QuizOption> { correctOption1, correctOption2, incorrectOption };
        _courseClientMock.Setup(x => x.GetQuizOptions(questionId)).ReturnsAsync(quizOptions);

        var request = new StudentSubmissionRequest
        {
            QuestionId = questionId,
            SubmissionData = new StudentSubmission
            {
                QuizSubmission = new List<Guid> { correctOption1.Id } // Only one correct selected
            }
        };

        // Act
        var result = await _quizEvaluationService.Evaluate(request);

        // Assert
        Assert.Equal(EvaluationStatus.Rejected, result.Status);
        _courseClientMock.Verify(x => x.GetQuizOptions(questionId), Times.Once);
    }

    [Fact]
    public async Task Evaluate_IncorrectOptionSelected_ReturnsRejected()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var correctOption = new QuizOption { Id = Guid.NewGuid(), Label = "ans", QuestionId = questionId, Correct = true };
        var incorrectOption = new QuizOption { Id = Guid.NewGuid(), Label = "ans", QuestionId = questionId, Correct = false };

        var quizOptions = new List<QuizOption> { correctOption, incorrectOption };
        _courseClientMock.Setup(x => x.GetQuizOptions(questionId)).ReturnsAsync(quizOptions);

        var request = new StudentSubmissionRequest
        {
            QuestionId = questionId,
            SubmissionData = new StudentSubmission
            {
                QuizSubmission = new List<Guid> { correctOption.Id, incorrectOption.Id } // Selected incorrect too
            }
        };

        // Act
        var result = await _quizEvaluationService.Evaluate(request);

        // Assert
        Assert.Equal(EvaluationStatus.Rejected, result.Status);
        _courseClientMock.Verify(x => x.GetQuizOptions(questionId), Times.Once);
    }
    
    [Fact]
    public async Task Evaluate_NullQuizSubmission_ReturnsRejectedWhenCorrectOptionsExist()
    {
        // Arrange
        var questionId = Guid.NewGuid();
        var correctOption = new QuizOption { Id = Guid.NewGuid(), Label = "ans", QuestionId = questionId, Correct = true };

        var quizOptions = new List<QuizOption> { correctOption };
        _courseClientMock.Setup(x => x.GetQuizOptions(questionId)).ReturnsAsync(quizOptions);

        var request = new StudentSubmissionRequest
        {
            QuestionId = questionId,
            SubmissionData = new StudentSubmission
            {
                QuizSubmission = null
            }
        };

        // Act
        var result = await _quizEvaluationService.Evaluate(request);

        // Assert
        Assert.Equal(EvaluationStatus.Rejected, result.Status);
        _courseClientMock.Verify(x => x.GetQuizOptions(questionId), Times.Once);
    }
}