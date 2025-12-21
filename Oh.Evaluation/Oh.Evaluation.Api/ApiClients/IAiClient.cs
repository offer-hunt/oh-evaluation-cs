using Contracts.Internal.Ai;
using Refit;

namespace Oh.Evaluation.Api.ApiClients;

public interface IAiClient
{
    [Post("/ai/review/code")]
    public Task<CodeReviewResponse> GetCodeReview([Body] CodeReviewRequest request);
    
    [Post("/ai/evaluate/text-answer")]
    public Task<TextReviewResponse> GetTextReview([Body] TextReviewRequest request);
}