using Contracts.Internal.Course;
using Refit;

namespace Oh.Evaluation.Api.ApiClients;

public interface ICourseClient
{
    [Get("/question/getById/{id}")]
    public Task<CourseQuestion> GetById(Guid id);
    
    [Get("/question/getAllOptionsByQuestionId/{questionId}")]
    public Task<IEnumerable<QuizOption>> GetQuizOptions(Guid questionId);
}