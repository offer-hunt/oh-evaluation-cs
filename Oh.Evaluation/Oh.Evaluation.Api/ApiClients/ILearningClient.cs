using Contracts.Internal.Learning;
using Refit;

namespace Oh.Evaluation.Api.ApiClients;

public interface ILearningClient
{
    [Post("/api/learning/ratings/{courseId}}")]
    public Task InsertEvaluationResult(Guid courseId, [Body] QuestionStateUpsertRequest request);
}