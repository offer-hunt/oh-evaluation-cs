using Contracts.Domain;
using Contracts.Front;
using Contracts.Internal.Course;
using Contracts.Internal.Learning;
using Microsoft.EntityFrameworkCore;
using Moq;
using Oh.Evaluation.Api.Abstractions;
using Oh.Evaluation.Api.ApiClients;
using Oh.Evaluation.Api.Services;
using Oh.Evaluation.Domain.Domain;
using Oh.Evaluation.Infrastructure;

namespace Oh.Evaluation.Tests;

public class StudentSubmissionServiceTests
{
    private readonly Mock<ICourseClient> _courseClient = new();
    private readonly Mock<ILearningClient> _learningClient = new();
    private readonly Mock<ITextEvaluationService> _textEvaluationService = new();
    private readonly Mock<IQuizEvaluationService> _quizEvaluationService = new();
    private readonly Mock<ICodeEvaluationService> _codeEvaluationService = new();
    private readonly Mock<IUserContextService> _userContextService = new();
    private readonly EvaluationDbContext _dbContext;

    public StudentSubmissionServiceTests()
    {
        var options = new DbContextOptionsBuilder<EvaluationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new EvaluationDbContext(options);
    }
    
    [Fact]
    public async Task GetResult_ReturnsCorrectData()
    {
        // Arrange
        var submission = new RunnerSubmission
        {
            Id = Guid.NewGuid(),
            Verdict = EvaluationStatus.Accepted,
            AiReview = new RunnerAiReview 
            { 
                Id = Guid.NewGuid(),
                Score = 100, 
                Suggestions = "Good job",
                Status = AiReviewStatus.Done,
                CreatedAt = DateTime.UtcNow
            },
            TestCaseResults = new List<RunnerTestCaseResult>
            {
                new() 
                { 
                    Id = Guid.NewGuid(), 
                    Status = TestCaseStatus.Pass 
                }
            }
        };
        
        await _dbContext.RunnerSubmissions.AddAsync(submission);
        await _dbContext.SaveChangesAsync();

        var service = CreateService();

        // Act
        var result = await service.GetResult(submission.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(EvaluationStatus.Accepted, result.Status);
        Assert.Equal(100, result.Score);
        Assert.Single(result.TestCaseResults!);
    }

    [Fact]
    public async Task SubmitTask_CreatesRunnerSubmissionAndSavesToDb()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userContextService.Setup(x => x.GetCurrentUserId()).Returns(userId);

        var request = new StudentSubmissionRequest
        {
            CourseId = Guid.NewGuid(),
            PageId = Guid.NewGuid(),
            QuestionId = Guid.NewGuid(),
            SubmissionData = new StudentSubmission
            {
                TextSubmission = "Test answer"
            }
        };

        _courseClient.Setup(x => x.GetById(request.QuestionId))
            .ReturnsAsync(new CourseQuestion 
            { 
                Id = request.QuestionId,
                PageId = request.PageId,
                Type = SubmissionType.TextInput,
                Text = "Test question",
                CorrectAnswer = "Test answer",
                UseAiCheck = false,
                Points = 10,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

        _textEvaluationService.Setup(x => x.Evaluate(request, It.IsAny<CourseQuestion>()))
            .ReturnsAsync(new SubmissionResult 
            { 
                Status = EvaluationStatus.Accepted,
                Score = 95,
                Feedback = new SubmissionFeedback
                {
                    Summary = "Good job",
                    Suggestions = "Keep it up"
                }
            });

        var service = CreateService();

        // Act
        var result = await service.SubmitTask(request);

        // Assert
        var submissionInDb = await _dbContext.RunnerSubmissions
            .Include(s => s.AiReview)
            .SingleAsync();
        Assert.Equal(userId, submissionInDb.UserId);
        Assert.Equal(EvaluationStatus.Accepted, submissionInDb.Verdict);
        Assert.Equal(95, submissionInDb.AiReview!.Score);
        Assert.Equal(95, result.Score);
        Assert.Equal(EvaluationStatus.Accepted, result.Status);

    }

    [Fact]
    public async Task SubmitTask_CallsLearningClient()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var pageId = Guid.NewGuid();
        var questionId = Guid.NewGuid();
        
        _userContextService.Setup(x => x.GetCurrentUserId()).Returns(userId);

        var request = new StudentSubmissionRequest
        {
            CourseId = courseId,
            PageId = pageId,
            QuestionId = questionId,
            SubmissionData = new StudentSubmission
            {
                TextSubmission = "Test answer"
            }
        };

        _courseClient.Setup(x => x.GetById(questionId))
            .ReturnsAsync(new CourseQuestion 
            { 
                Id = questionId,
                PageId = pageId,
                Type = SubmissionType.TextInput, 
                Text = "Test question",
                CorrectAnswer = "Correct answer",
                UseAiCheck = true,
                Points = 10,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

        _textEvaluationService.Setup(x => x.Evaluate(request, It.IsAny<CourseQuestion>()))
            .ReturnsAsync(new SubmissionResult 
            { 
                Status = EvaluationStatus.Accepted, 
                Score = 80, 
                Feedback = new SubmissionFeedback
                {
                    Summary = "Good",
                    Suggestions = "Improve"
                }
            });

        var service = CreateService();

        // Act
        await service.SubmitTask(request);

        // Assert
        _learningClient.Verify(x => x.InsertEvaluationResult(
            courseId,
            It.Is<QuestionStateUpsertRequest>(r => 
                r.Id == questionId &&
                r.UserId == userId &&
                r.Status == EvaluationStatus.Accepted && 
                r.Score == 80 &&
                r.Feedback == "Good"
            )
        ), Times.Once);
    }


    private StudentSubmissionService CreateService() =>
        new(_courseClient.Object,
            _learningClient.Object,
            _textEvaluationService.Object,
            _quizEvaluationService.Object,
            _codeEvaluationService.Object,
            _userContextService.Object,
            _dbContext);
}