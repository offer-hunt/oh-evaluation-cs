using Contracts.Internal.Learning;
using Refit;

namespace Oh.Evaluation.Api.ApiClients;

public interface ILearningClient
{
    [Post("/api/v1/internal/upsert-result")]
    public Task InsertEvaluationResult(Guid courseId, [Body] QuestionStateUpsertRequest request);
}