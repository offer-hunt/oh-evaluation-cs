using System.ComponentModel.DataAnnotations;
using Contracts.Domain;
using Contracts.Front;
using Contracts.Internal.Ai;
using Contracts.Internal.Course;
using Moq;
using Oh.Evaluation.Api.ApiClients;
using Oh.Evaluation.Api.Services;

namespace Oh.Evaluation.Tests
{
    public class TextEvaluationServiceTests
    {
        private readonly Mock<IAiClient> _aiClientMock;
        private readonly TextEvaluationService _textEvaluationService;

        public TextEvaluationServiceTests()
        {
            _aiClientMock = new Mock<IAiClient>();
            _textEvaluationService = new TextEvaluationService(_aiClientMock.Object);
        }

        [Fact]
        public async Task Evaluate_ExactMatch_ReturnsAcceptedWithoutAiCall()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var correctAnswer = "Correct answer";
            
            var request = new StudentSubmissionRequest
            {
                QuestionId = questionId,
                SubmissionData = new StudentSubmission
                {
                    TextSubmission = correctAnswer
                }
            };

            var question = new CourseQuestion
            {
                Id = questionId,
                Text = "Test question",
                CorrectAnswer = correctAnswer,
                UseAiCheck = true, // AI check is enabled but shouldn't be called
                Type = SubmissionType.TextInput
            };

            // Act
            var result = await _textEvaluationService.Evaluate(request, question);

            // Assert
            Assert.Equal(EvaluationStatus.Accepted, result.Status);
            Assert.Null(result.Score); // No score when exact match
            Assert.Null(result.Feedback);
            
            // Verify AI client was NOT called since answers match exactly
            _aiClientMock.Verify(
                x => x.GetTextReview(It.IsAny<TextReviewRequest>(), It.IsAny<CancellationToken>()), 
                Times.Never);
        }

        [Fact]
        public async Task Evaluate_NoMatch_AiCheckDisabled_ReturnsRejected()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var correctAnswer = "Correct answer";
            var studentAnswer = "Wrong answer";
            
            var request = new StudentSubmissionRequest
            {
                QuestionId = questionId,
                SubmissionData = new StudentSubmission
                {
                    TextSubmission = studentAnswer
                }
            };

            var question = new CourseQuestion
            {
                Id = questionId,
                Text = "Test question",
                CorrectAnswer = correctAnswer,
                UseAiCheck = false, // AI check disabled
                Type = SubmissionType.TextInput
            };

            // Act
            var result = await _textEvaluationService.Evaluate(request, question);

            // Assert
            Assert.Equal(EvaluationStatus.Rejected, result.Status);
            Assert.Null(result.Score);
            Assert.Null(result.Feedback);
            
            // Verify AI client was NOT called
            _aiClientMock.Verify(
                x => x.GetTextReview(It.IsAny<TextReviewRequest>(), default), 
                Times.Never);
        }

        [Fact]
        public async Task Evaluate_NoMatch_AiCheckEnabled_ReturnsRejectedWithAiFeedback()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var correctAnswer = "Correct answer with detailed explanation";
            var studentAnswer = "Partially correct answer";
            var aiScore = 75;
            var aiSummary = "Good attempt but missing details";
            var aiSuggestions = "Include more specific examples";
            
            var request = new StudentSubmissionRequest
            {
                QuestionId = questionId,
                SubmissionData = new StudentSubmission
                {
                    TextSubmission = studentAnswer
                }
            };

            var question = new CourseQuestion
            {
                Id = questionId,
                Text = "Explain the concept of XYZ",
                CorrectAnswer = correctAnswer,
                UseAiCheck = true, // AI check enabled
                Type = SubmissionType.TextInput
            };

            _aiClientMock
                .Setup(x => x.GetTextReview(It.Is<TextReviewRequest>(r => 
                    r.Answer == studentAnswer && 
                    r.Question == question.Text &&
                    r.Rubric == correctAnswer), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TextReviewResponse
                {
                    Score = aiScore,
                    Summary = aiSummary,
                    Suggestions = aiSuggestions
                });

            // Act
            var result = await _textEvaluationService.Evaluate(request, question);

            // Assert
            Assert.Equal(EvaluationStatus.Rejected, result.Status);
            Assert.Equal(aiScore, result.Score);
            Assert.NotNull(result.Feedback);
            Assert.Equal(aiSummary, result.Feedback.Summary);
            Assert.Equal(aiSuggestions, result.Feedback.Suggestions);
            
            // Verify AI client was called exactly once
            _aiClientMock.Verify(
                x => x.GetTextReview(It.IsAny<TextReviewRequest>(), It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Fact]
        public async Task Evaluate_EmptyAnswer_ThrowsValidationException()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            
            var request = new StudentSubmissionRequest
            {
                QuestionId = questionId,
                SubmissionData = new StudentSubmission
                {
                    TextSubmission = string.Empty // Empty answer
                }
            };

            var question = new CourseQuestion
            {
                Id = questionId,
                Text = "Test question",
                CorrectAnswer = "Correct answer",
                UseAiCheck = false,
                Type = SubmissionType.TextInput
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                () => _textEvaluationService.Evaluate(request, question));
            
            // Verify AI client was NOT called
            _aiClientMock.Verify(
                x => x.GetTextReview(It.IsAny<TextReviewRequest>(), It.IsAny<CancellationToken>()), 
                Times.Never);
        }

        [Fact]
        public async Task Evaluate_NullAnswer_ThrowsValidationException()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            
            var request = new StudentSubmissionRequest
            {
                QuestionId = questionId,
                SubmissionData = new StudentSubmission
                {
                    TextSubmission = null // Null answer
                }
            };

            var question = new CourseQuestion
            {
                Id = questionId,
                Text = "Test question",
                CorrectAnswer = "Correct answer",
                UseAiCheck = false,
                Type = SubmissionType.TextInput
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                () => _textEvaluationService.Evaluate(request, question));
            
            // Verify AI client was NOT called
            _aiClientMock.Verify(
                x => x.GetTextReview(It.IsAny<TextReviewRequest>(), It.IsAny<CancellationToken>()), 
                Times.Never);
        }

        [Fact]
        public async Task Evaluate_WhitespaceOnlyAnswer_ThrowsValidationException()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            
            var request = new StudentSubmissionRequest
            {
                QuestionId = questionId,
                SubmissionData = new StudentSubmission
                {
                    TextSubmission = "   " // Whitespace only
                }
            };

            var question = new CourseQuestion
            {
                Id = questionId,
                Text = "Test question",
                CorrectAnswer = "Correct answer",
                UseAiCheck = false,
                Type = SubmissionType.TextInput
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(
                () => _textEvaluationService.Evaluate(request, question));
            
            // Verify AI client was NOT called
            _aiClientMock.Verify(
                x => x.GetTextReview(It.IsAny<TextReviewRequest>(), It.IsAny<CancellationToken>()), 
                Times.Never);
        }

        [Fact]
        public async Task Evaluate_AiServiceThrowsException_PropagatesException()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var correctAnswer = "Correct answer";
            var studentAnswer = "Wrong answer";
            
            var request = new StudentSubmissionRequest
            {
                QuestionId = questionId,
                SubmissionData = new StudentSubmission
                {
                    TextSubmission = studentAnswer
                }
            };

            var question = new CourseQuestion
            {
                Id = questionId,
                Text = "Test question",
                CorrectAnswer = correctAnswer,
                UseAiCheck = true,
                Type = SubmissionType.TextInput
            };

            _aiClientMock
                .Setup(x => x.GetTextReview(It.IsAny<TextReviewRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("AI service unavailable"));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(
                () => _textEvaluationService.Evaluate(request, question));
        }
    }
}